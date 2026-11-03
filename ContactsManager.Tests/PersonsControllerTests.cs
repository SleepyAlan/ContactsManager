using AutoFixture;
using ContactsManager.Controllers;
using ContactsManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using Services;
using System.ComponentModel;
using System.Text;

namespace ContactsManager.Tests;

public class PersonsControllerTests
{
    private readonly PersonsController _sut;
    private readonly Mock<IPersonService> _personServiceMock;
    private readonly Mock<ICountryService> _countryServiceMock;
    private readonly IFixture _fixture;

    public PersonsControllerTests()
    {
        _fixture = new Fixture();
        _personServiceMock = new Mock<IPersonService>();
        _countryServiceMock = new Mock<ICountryService>();
        var nullLogger = new Microsoft.Extensions.Logging.Abstractions
                             .NullLogger<PersonsController>();
        _sut = new PersonsController(
            _countryServiceMock.Object, _personServiceMock.Object, nullLogger);
    }

    #region Index Tests

    [Fact]
    public async Task Index_IfPersonsExist_ReturnIndexViewWithPersonsViewModel()
    {
        // Arrange
        List<PersonResponse> persons = _fixture
            .CreateMany<PersonResponse>(10)
            .ToList();
        SetupPersonServiceMockCollectionMethods(persons);

        var searchField = _fixture.Create<SearchOptions?>();
        var searchString = _fixture.Create<string?>();
        var sortBy = _fixture.Create<FilterOptions>();
        var sortDirection = _fixture.Create<ListSortDirection>();

        // Act
        IActionResult result = await _sut.Index(searchField, searchString, sortBy, sortDirection);
        
        // Assert Types
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        PersonsViewModel personViewModel = Assert.IsType<PersonsViewModel>(viewResult.ViewData.Model);

        // Assert Values within view model
        Assert.Equal(persons.Count, personViewModel.Persons.Count());
        Assert.Equal(searchField.ToString(), personViewModel.SelectedSearchField);
        Assert.Equal(searchString, personViewModel.SearchString);
        Assert.Equal(sortBy.ToString(), personViewModel.SortColumn);
        Assert.Equal(sortDirection.ToString(), personViewModel.SortDirection);
    }

    [Fact]
    public async Task Index_IfNoPersonsExist_ReturnIndexViewWithEmptyCollectionInPersonsViewModel()
    {
        // Arrange
        List<PersonResponse> persons = new List<PersonResponse>();
        SetupPersonServiceMockCollectionMethods(persons);

        // Act
        IActionResult result = await _sut.Index(null, null);
        
        // Assert Types
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        PersonsViewModel personViewModel = Assert.IsType<PersonsViewModel>(viewResult.ViewData.Model);

        // Assert Values within view model
        Assert.Empty(personViewModel.Persons);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_HttpGetRequest_ReturnCreateViewResult()
    {
        // Arrange
        SetupCountryServiceMock(new List<CountryResponse>());

        // Act
        IActionResult result = await _sut.Create();

        // Assert Types
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_HttpGetRequest_ReturnsViewWithGenderAndCountriesPopulated()
    {
        // Arrange
        var expectedCountries = _fixture.CreateMany<CountryResponse>(5).ToList();
        SetupCountryServiceMock(expectedCountries);
        var genders = Enum.GetNames(typeof(GenderOptions));
        var expectedGenderItems = genders.Select(g => new SelectListItem
        {
            Value = g,
            Text = g
        });
        var expectedCountryItems = expectedCountries.Select(c => new SelectListItem
        {
            Value = c.CountryId.ToString(),
            Text = c.CountryName
        });

        // Act
        IActionResult result = await _sut.Create();

        // Assert Types
        ViewResult viewResult = Assert.IsType<ViewResult>(result);

        // Assert ViewBag Values
        var actualCountryItems = viewResult.ViewData["CountryItems"] as IEnumerable<SelectListItem>;
        var actualGenderItems = viewResult.ViewData["GenderItems"] as IEnumerable<SelectListItem>;

        Assert.Equal(
            expectedCountryItems.Select(c => new { c.Value, c.Text }),
            actualCountryItems?.Select(c => new { c.Value, c.Text }));
        Assert.Equal(
            expectedGenderItems.Select(g => new { g.Value, g.Text }),
            actualGenderItems?.Select(c => new { c.Value, c.Text }));
    }

    [Fact]
    public async Task Create_HttpPostRequestWithValidModel_PersonAddedAndRedirectToIndexView()
    {
        // Arrange
        PersonAddRequest request = _fixture.Create<PersonAddRequest>();
        _personServiceMock.Setup(s => s.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(_fixture.Create<PersonResponse>());

        // Act
        IActionResult result = await _sut.Create(request);

        // Assert Types
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _personServiceMock.Verify(
            s => s.AddPerson(It.IsAny<PersonAddRequest>()), Times.Once);
    }

    [Fact]
    public async Task Create_HttpPostRequestWithInvalidModel_ReturnsCreateViewWithTheInvalidModel()   {
        // Arrange
        PersonAddRequest request = _fixture.Create<PersonAddRequest>();
        SetupCountryServiceMock(new List<CountryResponse>());
        _personServiceMock.Setup(s => s.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(_fixture.Create<PersonResponse>());

        // Act
        _sut.ModelState.AddModelError("Name", "Name is required");
        IActionResult result = await _sut.Create(request);

        // Assert Types
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(request, viewResult.Model);
        _personServiceMock.Verify(
            s => s.AddPerson(It.IsAny<PersonAddRequest>()), Times.Never);
    }
    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_HttpGetRequestWithValidPersonId_ReturnsUpdateViewWithPersonUpdateRequest()
    {
        // Arrange
        SetupCountryServiceMock(new List<CountryResponse>());
        var personResponse = SetupPersonServiceMockCRUDMethods();

        // Act
        IActionResult result = await _sut.Update(personResponse.PersonId);

        // Assert Types
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        PersonUpdateRequest updateRequest = Assert.IsType<PersonUpdateRequest>(viewResult.Model);
        Assert.Equal(personResponse.PersonId, updateRequest.PersonId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("9b01639d-27a9-467a-bc12-f3ab438753c9")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Update_HttpGetRequestWithInvalidPersonId_ReturnsNotFoundResult(string? id)
    {
        // Arrange
        Guid? personId = string.IsNullOrEmpty(id) ? null : Guid.Parse(id);
        SetupCountryServiceMock(new List<CountryResponse>());
        _personServiceMock
            .Setup(service => service.GetPersonById(It.IsAny<Guid?>()))
            .ReturnsAsync((PersonResponse?)null);

        // Act
        IActionResult result = await _sut.Update(personId);

        // Assert Types
        NotFoundResult notFoundResult = Assert.IsType<NotFoundResult>(result);

    }

    [Fact]
    public async Task Update_HttpGetRequestWithExceptionThrownByGetPersonById_ReturnsRedirectToIndexView()
    {
        // Arrange
        _personServiceMock
            .Setup(service => service.GetPersonById(It.IsAny<Guid?>()))
            .ThrowsAsync(new ArgumentNullException());

        // Act
        IActionResult result = await _sut.Update(Guid.NewGuid());

        // Assert Types
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Update_HttpPostRequestWithValidModel_PersonUpdatedAndRedirectToIndexView()
    {
        // Arrange
        PersonUpdateRequest request = _fixture.Create<PersonUpdateRequest>();
        _personServiceMock.Setup(s => s.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
            .ReturnsAsync(_fixture.Create<PersonResponse>());

        // Act
        IActionResult result = await _sut.Update(request);

        // Assert Types
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _personServiceMock.Verify(
            s => s.UpdatePerson(It.IsAny<PersonUpdateRequest>()), Times.Once);
    }

    [Fact]
    public async Task Update_HttpPostRequestWithInvalidModel_ReturnsUpdateViewWithTheInvalidModel()
    {
        // Arrange
        PersonUpdateRequest request = _fixture.Create<PersonUpdateRequest>();
        SetupCountryServiceMock(new List<CountryResponse>());
        _personServiceMock.Setup(s => s.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
            .ReturnsAsync(_fixture.Create<PersonResponse>());

        // Act
        _sut.ModelState.AddModelError("Name", "Name is required");
        IActionResult result = await _sut.Update(request);

        // Assert Types
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(request, viewResult.Model);
        _personServiceMock.Verify(
            s => s.UpdatePerson(It.IsAny<PersonUpdateRequest>()), Times.Never);
    }

    [Fact]
    public async Task Update_HttpPostRequestWithExceptionThrownByUpdatePerson_ReturnsBadRequest()
    {
        // Arrange
        PersonUpdateRequest request = _fixture.Create<PersonUpdateRequest>();
        _personServiceMock.Setup(s => s.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
            .ThrowsAsync(new ArgumentNullException());
        // Act
        IActionResult result = await _sut.Update(request);
        // Assert Types
        BadRequestResult badRequestResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }
    #endregion

    #region Details Tests

    [Fact]
    public async Task Details_HttpGetRequestWithValidPersonId_ReturnsDetailsViewWithPersonResponse()
    {
        // Arrange
        var personResponse = SetupPersonServiceMockCRUDMethods();

        // Act
        IActionResult result = await _sut.Details(personResponse.PersonId);

        // Assert Types
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        PersonResponse actualPersonResponse = Assert.IsType<PersonResponse>(viewResult.Model);
        Assert.Equal(personResponse.PersonId, actualPersonResponse.PersonId);
    }

    [Fact]
    public async Task Details_HttpGetRequestWithInvalidPersonId_ReturnsNotFoundResult()
    {
        // Arrange
        _personServiceMock
            .Setup(service => service.GetPersonById(It.IsAny<Guid?>()))
            .ReturnsAsync((PersonResponse?)null);

        // Act
        IActionResult result = await _sut.Details(null);

        // Assert Types
        NotFoundResult notFoundResult = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_HttpGetRequestWithExceptionThrownByGetPersonById_ReturnsNotFoundResult()
    {
        // Arrange
        _personServiceMock
            .Setup(service => service.GetPersonById(It.IsAny<Guid?>()))
            .ThrowsAsync(new ArgumentNullException());
        // Act
        IActionResult result = await _sut.Details(Guid.NewGuid());

        // Assert Types
        NotFoundResult notFoundResult = Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_HttpPostRequestWithValidPersonId_PersonDeletedAndRedirectToIndexView()
    {
        // Arrange
        var personResponse = SetupPersonServiceMockCRUDMethods();

        // Act
        IActionResult result = await _sut.Delete(personResponse.PersonId);

        // Assert Types
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _personServiceMock.Verify(
            s => s.DeletePerson(It.IsAny<Guid>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("9b01639d-27a9-467a-bc12-f3ab438753c9")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Delete_HttpPostRequestWithInvalidPersonId_ReturnsRedirectToIndexView(string?  id)
    {
        // Arrange
        Guid? personId = string.IsNullOrEmpty(id) ? null : Guid.Parse(id);
        SetupPersonServiceMockCRUDMethods();

        // Act
        IActionResult result = await _sut.Delete(personId);

        // Assert Types
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
    #endregion

    #region Table Tests

    [Fact]
    public async Task Table_HttpGetRequest_ReturnsTablePartialViewWithPersonsViewModel()
    {
        // Arrange
        List<PersonResponse> persons = _fixture
            .CreateMany<PersonResponse>(10)
            .ToList();
        SetupPersonServiceMockCollectionMethods(persons);
        var searchField = _fixture.Create<SearchOptions?>();
        var searchString = _fixture.Create<string?>();
        var sortBy = _fixture.Create<FilterOptions>();
        var sortDirection = _fixture.Create<ListSortDirection>();

        // Act
        IActionResult result = await _sut.Table(searchField, searchString, sortBy, sortDirection);

        // Assert Types
        PartialViewResult viewResult = Assert.IsType<PartialViewResult>(result);
        PersonsViewModel personViewModel = Assert.IsType<PersonsViewModel>(viewResult.ViewData.Model);

        // Assert Values within view model
        Assert.Equal(persons.Count, personViewModel.Persons.Count());
        Assert.Equal(searchField.ToString(), personViewModel.SelectedSearchField);
        Assert.Equal(searchString, personViewModel.SearchString);
        Assert.Equal(sortBy.ToString(), personViewModel.SortColumn);
        Assert.Equal(sortDirection.ToString(), personViewModel.SortDirection);
    }

    #endregion

    #region PersonsPDF Tests

    [Fact]
    public async Task PersonsPDF_HttpGetRequest_ReturnsViewAsPdf()
    {
        // Arrange
        List<PersonResponse> persons = _fixture
            .CreateMany<PersonResponse>(10)
            .ToList();
        SetupPersonServiceMockCollectionMethods(persons);
        var searchField = _fixture.Create<SearchOptions?>();
        var searchString = _fixture.Create<string?>();
        var sortBy = _fixture.Create<FilterOptions>();
        var sortDirection = _fixture.Create<ListSortDirection>();

        // Act
        IActionResult result = await _sut.PersonsPDF(searchField, searchString, sortBy, sortDirection);

        // Assert Types
        ViewAsPdf pdfResult = Assert.IsType<ViewAsPdf>(result);
        Assert.Equal("ContactsTablePdf", pdfResult.ViewName);
        Assert.Equal("Contacts.pdf", pdfResult.FileName);
    }

    [Fact]
    public async Task PersonsPDF_HttpGetRequest_ReturnsViewAsPdfWithCorrectOptions()
    {
        // Arrange
        SetupPersonServiceMockCollectionMethods([]);

        // Act
        IActionResult result = await _sut.PersonsPDF(null, null);

        // Assert Types
        ViewAsPdf pdfResult = Assert.IsType<ViewAsPdf>(result);

        Assert.Equal(Size.A4, pdfResult.PageSize);
        Assert.Equal(Orientation.Landscape, pdfResult.PageOrientation);

        Assert.Equal(12, pdfResult.PageMargins.Top);
        Assert.Equal(10, pdfResult.PageMargins.Right);
        Assert.Equal(12, pdfResult.PageMargins.Bottom);
        Assert.Equal(10, pdfResult.PageMargins.Left);
    }

    [Fact]
    public async Task PersonsPDF_HttpGetRequest_ReturnsViewAsPdfWithCorrectModel()
    {
        // Arrange
        List<PersonResponse> expectedPersons = _fixture
            .CreateMany<PersonResponse>(10)
            .ToList();
        SetupPersonServiceMockCollectionMethods(expectedPersons);

        // Act
        IActionResult result = await _sut.PersonsPDF(null, null);

        // Assert Types
        ViewAsPdf pdfResult = Assert.IsType<ViewAsPdf>(result);

        var model = Assert.IsType<PersonsViewModel>(pdfResult.Model);

        Assert.Equal(expectedPersons, model.Persons);
    }

    #endregion

    #region PersonsCSV Tests

    [Fact]
    public async Task PersonsCSV_HttpGetRequest_ReturnsFileContentResultWithCorrectContentTypeAndFileName()
    {
        // Arrange
        List<PersonResponse> persons = _fixture
            .CreateMany<PersonResponse>(10)
            .ToList();
        SetupPersonServiceMockCollectionMethods(persons);
        var csvContent = "Name,Email,Phone\nJohn Doe,john.doe@example.com,123-456-7890";
        var bytes = Encoding.UTF8.GetBytes(csvContent);
        _personServiceMock
            .Setup(service => service.GetPersonsCsvFile(It.IsAny<IEnumerable<PersonResponse>?>()))
            .ReturnsAsync(new MemoryStream(bytes));

        // Act
        IActionResult result = await _sut.PersonsCSV(null, null);

        // Assert Types
        FileStreamResult fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Equal("Contacts.csv", fileResult.FileDownloadName);
        Assert.Equal(csvContent, Encoding.UTF8.GetString(((MemoryStream)fileResult.FileStream).ToArray()));
    }

    #endregion

    #region PersonsExcel Tests

    [Fact]
    public async Task PersonsExcel_HttpGetRequest_ReturnsFileContentResultWithCorrectContentTypeAndFileName()
    {
        // Arrange
        List<PersonResponse> persons = _fixture
            .CreateMany<PersonResponse>(10)
            .ToList();
        SetupPersonServiceMockCollectionMethods(persons);
        var excelContent = "Name,Email,Phone\nJohn Doe,john.doe@example.com,123-456-7890";
        var bytes = Encoding.UTF8.GetBytes(excelContent);
        _personServiceMock
            .Setup(service => service.GetPersonsExcelFile(It.IsAny<IEnumerable<PersonResponse>?>()))
            .ReturnsAsync(new MemoryStream(bytes));

        // Act
        IActionResult result = await _sut.PersonsExcel(null, null);

        // Assert Types
        FileStreamResult fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
        Assert.Equal("Contacts.xlsx", fileResult.FileDownloadName);
        Assert.Equal(excelContent, Encoding.UTF8.GetString(((MemoryStream)fileResult.FileStream).ToArray()));
    }

    #endregion

    #region Helper Methods

    private void SetupPersonServiceMockCollectionMethods(
        List<PersonResponse> persons)
    {
        _personServiceMock
            .Setup(service => service.GetFilteredPersons(
                It.IsAny<string?>(),
                It.IsAny<SearchOptions>()))
            .ReturnsAsync(persons);
        _personServiceMock
            .Setup(service => service.GetSortedPersons(
                It.IsAny<IEnumerable<PersonResponse>>(),
                It.IsAny<FilterOptions>(),
                It.IsAny<ListSortDirection>()))
            .Returns(persons);
    }

    private PersonResponse SetupPersonServiceMockCRUDMethods(
        PersonAddRequest? personAddRequest = null,
        PersonResponse? personResponse = null,
        PersonUpdateRequest? personUpdateRequest = null)
    {
        personAddRequest ??= _fixture
            .Build<PersonAddRequest>()
            .With(p => p.Email, "test@example.com")
            .Create();
        personResponse ??= _fixture
            .Build<PersonResponse>()
            .With(p => p.Email, "test@example.com")
            .Create();
        personUpdateRequest ??= _fixture
            .Build<PersonUpdateRequest>()
            .With(p => p.Email, "test@example.com")
            .Create();

        _personServiceMock
            .Setup(service => service.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);
        _personServiceMock
            .Setup(service => service.AddPerson(null))
            .ThrowsAsync(new ArgumentNullException(nameof(personAddRequest)));

        _personServiceMock
            .Setup(service => service.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
            .ReturnsAsync(personResponse);
        _personServiceMock
            .Setup(service => service.UpdatePerson(null))
            .ThrowsAsync(new ArgumentNullException(nameof(personUpdateRequest)));

        _personServiceMock
            .Setup(service => service.GetPersonById(It.IsAny<Guid>()))
            .ReturnsAsync(personResponse);
        _personServiceMock
            .Setup(service => service.GetPersonById(null))
            .ReturnsAsync((PersonResponse?)null);

        _personServiceMock
            .Setup(service => service.DeletePerson(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _personServiceMock
            .Setup(service => service.DeletePerson(null))
            .ThrowsAsync(new ArgumentNullException("Invalid id"));

        return personResponse;
    }

    private void SetupCountryServiceMock(
        List<CountryResponse> countries)
    {
        _countryServiceMock
            .Setup(service => service.GetAllCountries())
            .ReturnsAsync(countries);
    }

    #endregion
}
