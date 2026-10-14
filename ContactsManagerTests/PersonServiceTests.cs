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

    public PersonServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _sut = new PersonService();
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
        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "test1",
            Email = "test1@example.com",
            Address = "sample address 1",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Now,
            ReceiveNewsLetters = true,
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "test2",
            Email = "test2@example.com",
            Address = "sample address 2",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Female,
            DateOfBirth = DateTime.Now,
            ReceiveNewsLetters = false,
        };
        List<PersonResponse> expectedPersonResponses = new List<PersonResponse>
        {
            _sut.AddPerson(personAddRequest1),
            _sut.AddPerson(personAddRequest2)
        };
        _testOutputHelper.WriteLine("Expected:");
        foreach(var expectedPersonResponse in expectedPersonResponses)
        {
            _testOutputHelper.WriteLine(expectedPersonResponse.ToString());
        }

        // Act
        List<PersonResponse> actualPersonResponses = _sut.GetAllPersons();
        _testOutputHelper.WriteLine("Actual:");
        foreach (var actualPersonResponse in actualPersonResponses)
        {
            _testOutputHelper.WriteLine(actualPersonResponse.ToString());
        }

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
}
