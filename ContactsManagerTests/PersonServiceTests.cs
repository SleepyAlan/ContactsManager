using ServiceContracts;
using Services;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;

namespace CRUDTests;

public class PersonServiceTests
{
    private readonly IPersonService _sut;

    public PersonServiceTests()
    {
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
    #endregion
}
