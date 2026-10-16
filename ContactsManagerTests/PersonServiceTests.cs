using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CRUDTests;

public class PersonServiceTests
{
    private readonly IPersonService _sut;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ICountryService _countryService;

    public PersonServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _countryService = new CountryService();
        _sut = new PersonService(_countryService);
    }

    #region AddPerson() Tests
    [Fact]
    public void AddPerson_NullPersonAddRequest_ThrowsArgumentNullException()
    {
        // Arrange
        PersonAddRequest? personAddRequest = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _sut.AddPerson(personAddRequest));
    }

    [Fact]
    public void AddPerson_PersonNameIsNull_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = null
        };
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _sut.AddPerson(personAddRequest));
    }

    [Fact]
    public void AddPerson_PersonNameIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = ""
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _sut.AddPerson(personAddRequest));
    }

    [Fact]
    public void AddPerson_PersonNameIsWhitespace_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = " "
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _sut.AddPerson(personAddRequest));
    }

    [Fact]
    public void AddPerson_ValidPersonAddRequest_ReturnsPersonResponse()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = "test",
            Email = "test@example.com",
            Address = "sample address",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Now,
            ReceiveNewsLetters = true,
        };
        // Act
        PersonResponse actualPersonResponse = _sut.AddPerson(personAddRequest);

        // Assert
        Assert.True(actualPersonResponse.PersonId != Guid.Empty);

    }

    [Fact]
    public void AddPerson_ValidPersonAddRequest_VerifyItPersistsViaGetAllPersons()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = "test",
            Email = "test@example.com",
            Address = "sample address",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.UtcNow,
            ReceiveNewsLetters = true,
        };

        // Act
        PersonResponse expectedPersonResponse = _sut.AddPerson(personAddRequest);
        List<PersonResponse> actualPersonResponses = _sut.GetAllPersons();


        // Assert
        Assert.Contains(expectedPersonResponse, actualPersonResponses);
    }

    #endregion

    #region GetAllPersons() Tests
    [Fact]
    public void GetAllPersons_IfNoPersons_ReturnsEmptyList()
    {
        // Act
        List<PersonResponse> actualPersonResponses = _sut.GetAllPersons();

        // Assert
        Assert.Empty(actualPersonResponses);
    }

    [Fact]
    public void GetAllPersons_IfFewPersons_ReturnsListOfPersons()
    {
        // Arrange
        List<PersonResponse> expectedPersonResponses = AddSamplePersons();
        PrintPersonResponses(expectedPersonResponses, "Expected:");

        // Act
        List<PersonResponse> actualPersonResponses = _sut.GetAllPersons();
        PrintPersonResponses(actualPersonResponses, "Actual:");

        // Assert
        foreach (var expectedPersonResponse in expectedPersonResponses)
        {
            Assert.Contains(expectedPersonResponse, actualPersonResponses);

        }
    }
    #endregion

    #region GetPersonById() Tests
    [Fact]
    public void GetPersonById_NullPersonId_ReturnsNull()
    {
        // Arrange
        Guid? personId = null;

        // Act
        PersonResponse? response = _sut.GetPersonById(personId);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public void GetPersonById_EmptyPersonId_ReturnsNull()
    {
        // Arrange
        Guid? personId = Guid.Empty;

        // Act
        PersonResponse? response = _sut.GetPersonById(personId);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public void GetPersonById_InvalidPersonId_ReturnsNull()
    {
        // Arrange
        Guid? personId = Guid.NewGuid();

        // Act
        PersonResponse? response = _sut.GetPersonById(personId);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public void GetPersonById_ValidPersonId_ReturnsPersonResponse()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = "test",
            Email = "test@example.com",
            Address = "sample address",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Now,
            ReceiveNewsLetters = true,
        };
        PersonResponse expectedPersonResponse = _sut.AddPerson(personAddRequest);

        // Act
        PersonResponse? actualPersonResponse =
            _sut.GetPersonById(expectedPersonResponse.PersonId);

        // Assert
        Assert.NotNull(actualPersonResponse);
        Assert.Equal(expectedPersonResponse, actualPersonResponse);
    }
    #endregion

    #region GetFilteredPersons() Tests
    [Fact]
    public void GetFilteredPersons_IfNoPersons_ReturnsEmptyList()
    {
        // Act
        List<PersonResponse> actualPersonResponses = _sut.GetFilteredPersons(null, SearchOptions.None);

        // Assert
        Assert.Empty(actualPersonResponses);
    }

    [Fact]
    public void GetFilteredPersons_FewPersonsAdded_NoFilter_ReturnsAllPersonResponses()
    {
        // Arrange
        List<PersonResponse> expectedPersonResponses = AddSamplePersons();
        PrintPersonResponses(expectedPersonResponses, "Expected:");

        // Act
        List<PersonResponse> actualPersonResponses =
               _sut.GetFilteredPersons(null, SearchOptions.None);
        PrintPersonResponses(actualPersonResponses, "Actual:");

        // Assert
        foreach (var expectedPersonResponse in expectedPersonResponses)
        {
            Assert.Contains(expectedPersonResponse, actualPersonResponses);

        }
    }

    [Fact]
    public void GetFilteredPersons_FilterByCountryUSA_ReturnsExactly2ValidPersonResponses()
    {
        // Arrange
        AddSamplePersons(); // Contains 2 persons with country as "Canada"
        const string countryName = "Canada";

        // Act
        List<PersonResponse> actualPersonResponses =
               _sut.GetFilteredPersons(countryName, SearchOptions.ByCountry);
        PrintPersonResponses(actualPersonResponses, "Actual:");
        var all = _sut.GetAllPersons();

        // Assert
        Assert.Equal(2, actualPersonResponses.Count);
        foreach (var actualPersonResponse in actualPersonResponses)
        {
            Assert.Contains(countryName, actualPersonResponse.CountryName, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void GetFilteredPersons_FilterByPersonName_ReturnsExactly1ValidPersonResponses()
    {
        // Arrange
        AddSamplePersons(); // Contains 1 person with name that has substring "Alan"
        const string personName = "Alan";

        // Act
        List<PersonResponse> actualPersonResponses =
               _sut.GetFilteredPersons(personName, SearchOptions.ByName);
        PrintPersonResponses(actualPersonResponses, "Actual:");

        // Assert
        Assert.Single(actualPersonResponses);
        foreach (var actualPersonResponse in actualPersonResponses)
        {
            Assert.Contains(personName, actualPersonResponse.PersonName, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void GetFilteredPersons_FilterByGender_ReturnsExactly2ValidPersonResponses()
    {
        // Arrange
        AddSamplePersons(); // Contains 2 persons with gender as "Male"
        const string gender = "Male";

        // Act
        List<PersonResponse> actualPersonResponses =
               _sut.GetFilteredPersons(gender, SearchOptions.ByGender);
        PrintPersonResponses(actualPersonResponses, "Actual:");

        // Assert
        Assert.Equal(2, actualPersonResponses.Count);
        foreach (var actualPersonResponse in actualPersonResponses)
        {
            Assert.Contains(gender, actualPersonResponse.Gender, StringComparison.OrdinalIgnoreCase);
        }
    }
    #endregion

    #region Helper Methods
    private void PrintPersonResponses(List<PersonResponse> personResponses, string title)
    {
        _testOutputHelper.WriteLine(title);
        foreach (var personResponse in personResponses)
        {
            _testOutputHelper.WriteLine(personResponse.ToString());
        }
    }

    private List<PersonResponse> AddSamplePersons()
    {
        CountryResponse countryUSA = _countryService.AddCountry(new CountryAddRequest() { CountryName = "USA" });
        CountryResponse countryCanada = _countryService.AddCountry(new CountryAddRequest() { CountryName = "Canada" });

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "Alan",
            Email = "alan@example.com",
            Address = "sample address 1",
            CountryId = countryUSA.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = new DateTime(2007, 1, 1),
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "Adam",
            Email = "adam@example.com",
            Address = "sample address 2",
            CountryId = countryCanada.CountryId,
            Gender = GenderOptions.Female,
            DateOfBirth = new DateTime(2011, 5, 1),
            ReceiveNewsLetters = false,
        };
        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            PersonName = "test3",
            Email = "test3@example.com",
            Address = "sample address 3",
            CountryId = null,
            Gender = GenderOptions.Male,
            DateOfBirth = new DateTime(1980, 1, 1),
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest4 = new PersonAddRequest()
        {
            PersonName = "test4",
            Email = "test4@example.com",
            Address = "sample address 4",
            CountryId = countryCanada.CountryId,
            Gender = GenderOptions.Other,
            DateOfBirth = new DateTime(2000, 1, 1),
            ReceiveNewsLetters = false,
        };
        List<PersonResponse> expectedPersonResponses = new List<PersonResponse>
        {
            _sut.AddPerson(personAddRequest1),
            _sut.AddPerson(personAddRequest2),
            _sut.AddPerson(personAddRequest3),
            _sut.AddPerson(personAddRequest4)
        };
        return expectedPersonResponses;
    }
    #endregion
}
