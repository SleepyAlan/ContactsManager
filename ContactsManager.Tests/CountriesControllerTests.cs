using AutoFixture;
using ContactsManager.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Extensions;

namespace ContactsManager.Tests;

public class CountriesControllerTests
{
    private readonly CountriesController _sut;
    private readonly Mock<ICountryService> _countryServiceMock;
    private readonly IFixture _fixture;

    public CountriesControllerTests()
    {
        _fixture = new Fixture();
        _countryServiceMock = new Mock<ICountryService>();
        _sut = new CountriesController(_countryServiceMock.Object);
    }

    #region AddCountry Tests

    [Fact]
    public void AddCountry_HttpGetRequest_ReturnsViewResult()
    {
        // Act
        IActionResult result = _sut.AddCountry();

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        // When view name is null, it means it will use the default view for the action
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public async Task AddCountry_HttpPostRequestWithValidModel_ReturnsViewResultWithSuccessMessage()
    {
        // Arrange
        var countryAddRequest = _fixture.Create<CountryAddRequest>();
        var countyResponse = countryAddRequest
            .ToCountry().ToCountryResponse();

        _countryServiceMock
            .Setup(service => service.AddCountry(countryAddRequest))
            .ReturnsAsync(countyResponse);

        // Act
        IActionResult result = await _sut.AddCountry(countryAddRequest);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
        Assert.NotNull(viewResult.ViewData["Message"]);
    }

    [Fact]
    public async Task AddCountry_HttpPostRequestWithInvalidModel_ReturnsViewResultWithModelError()
    {
        // Arrange
        var countryAddRequest = _fixture.Create<CountryAddRequest>();
        _sut.ModelState.AddModelError("CountryName", "Country name is required.");

        // Act
        IActionResult result = await _sut.AddCountry(countryAddRequest);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
        Assert.False(viewResult.ViewData.ModelState.IsValid);
    }

    [Fact]
    public async Task AddCountry_HttpPostRequestWithDuplicateCountryName_ReturnsViewResultWithModelError()
    {
        // Arrange
        var countryAddRequest = _fixture.Create<CountryAddRequest>();
        _countryServiceMock
            .Setup(service => service.AddCountry(countryAddRequest))
            .ThrowsAsync(new ArgumentException("Country name already exists."));

        // Act
        IActionResult result = await _sut.AddCountry(countryAddRequest);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
        Assert.False(viewResult.ViewData.ModelState.IsValid);
        Assert.True(viewResult.ViewData.ModelState.ContainsKey("CountryName"));
    }

    #endregion

    #region UploadCountriesFromExcel Tests

    [Fact]
    public async Task UploadCountriesFromExcel_HttpPostRequestWithValidExcelFile_ReturnsViewResultWithSuccessMessage()
    {
        // Arrange
        var excelFileMock = new Mock<IFormFile>();
        excelFileMock.Setup(f => f.Length).Returns(1);
        excelFileMock.Setup(f => f.FileName).Returns("countries.xlsx");
        _countryServiceMock
            .Setup(service => service.UploadCountriesFromExcel(excelFileMock.Object))
            .ReturnsAsync(5);

        // Act
        IActionResult result = await _sut.UploadCountriesFromExcel(excelFileMock.Object);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("AddCountry", viewResult.ViewName);
        Assert.NotNull(viewResult.ViewData["Message"]);
    }

    [Fact]
    public async Task UploadCountriesFromExcel_HttpPostRequestWithInvalidFile_ReturnsViewResultWithModelError()
    {
        // Arrange
        var invalidFileMock = new Mock<IFormFile>();
        invalidFileMock.Setup(f => f.Length).Returns(0);
        invalidFileMock.Setup(f => f.FileName).Returns("invalid.txt");
        // Act
        IActionResult result = await _sut.UploadCountriesFromExcel(invalidFileMock.Object);
        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("AddCountry", viewResult.ViewName);
        Assert.False(viewResult.ViewData.ModelState.IsValid);
        Assert.True(viewResult.ViewData.ModelState.ContainsKey("excelFile"));
    }

    [Fact]
    public async Task UploadCountriesFromExcel_HttpPostRequestWithNullFile_ReturnsViewResultWithModelError()
    {
        // Act
        IActionResult result = await _sut.UploadCountriesFromExcel(null);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("AddCountry", viewResult.ViewName);
        Assert.False(viewResult.ViewData.ModelState.IsValid);
        Assert.True(viewResult.ViewData.ModelState.ContainsKey("excelFile"));
    }

    #endregion
}
