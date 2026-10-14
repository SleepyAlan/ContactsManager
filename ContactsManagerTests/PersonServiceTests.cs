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
    public void AddPerson_ValidPersonAddRequest_ReturnsPersonResponse()
    {
        // Arrange
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = "test",
            Email = "test@example.com",
            Address = "sample address",
            CountryId = Guid.NewGuid(),
            Ser
        };
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _sut.AddPerson(personAddRequest));
    }


    #endregion

    #region GetAllPersons() Tests
    #endregion
}
