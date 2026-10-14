using ServiceContracts;
using Entities;
using ServiceContracts.DTOs;
using Services;

namespace ContactsManagerTests;

public class CountryServiceTests
{
    /// <summary>
    /// The System Under Test (SUT) for the CountriesService.
    /// </summary>
    private readonly ICountryService _sut;

    public CountryServiceTests()
    {
        _sut = new CountryService();
    }

    #region AddCountry Tests
    [Fact]
    public void AddCountry_NullCountryAddRequest_ThrowsArgumentNullException()
    {
        // Arrange
        CountryAddRequest? request = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _sut.AddCountry(request));
    }

    [Fact]
    public void AddCountry_NullCountryName_ThrowsArgumentException()
    {
        // Arrange
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = null
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => _sut.AddCountry(request));
    }

    [Fact]
    public void AddCountry_CountryNameIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = ""
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => _sut.AddCountry(request));
    }

    [Fact]
    public void AddCountry_CountryNameIsWhiteSpace_ThrowsArgumentException()
    {
        // Arrange
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = "   "
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => _sut.AddCountry(request));
    }

    [Fact]
    public void AddCountry_CountryNameIsDuplicate_ThrowsArgumentException()
    {
        // Arrange
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "USA"
        };
        CountryAddRequest request2 = new CountryAddRequest()
        {
            CountryName = "USA"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            _sut.AddCountry(request1);
            _sut.AddCountry(request2);
        });
    }

    [Fact]
    public void AddCountry_ValidCountryAddRequest_ReturnsCountryResponse()
    {
        // Arrange
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "Japan"
        };

        // Act
        CountryResponse countryResponse = _sut.AddCountry(request1);

        // Assert
        Assert.NotNull(countryResponse);
        Assert.True(countryResponse.CountryId != Guid.Empty);
        Assert.Equal(request1.CountryName, countryResponse.CountryName);
    }

    [Fact]
    public void AddCountry_ValidCountryAddRequest_VerifyIfValuePersistedViaGetAllCountries()
    {
        // Arrange
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "Japan"
        };

        // Act
        CountryResponse countryResponse = _sut.AddCountry(request1);
        List<CountryResponse> countries = _sut.GetAllCountries();

        // Assert
        Assert.Contains(countryResponse, countries);
    }
    #endregion

    #region GetAllCountries Tests
    [Fact]
    public void GetAllCountries_NoCountriesAvailable_ReturnsEmptyList()
    {
        // Act
        List<CountryResponse> countries = _sut.GetAllCountries();

        // Assert
        Assert.Empty(countries);
    }

    [Fact]
    public void GetAllCountries_FewCountriesAdded_ReturnsAllCountriesAdded()
    {
        // Arrange
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "Japan"
        };
        CountryAddRequest request2 = new CountryAddRequest()
        {
            CountryName = "USA"
        };
        List<CountryResponse> expectedCountries = new()
        {
            _sut.AddCountry(request1),
            _sut.AddCountry(request2)
        };

        // Act
        List<CountryResponse> actualCountries = _sut.GetAllCountries();

        // Assert
        Assert.Equal(2, actualCountries.Count);
        foreach (CountryResponse expectedCountry in expectedCountries)
        {
            Assert.Contains(expectedCountry, actualCountries);
        }
    }
    #endregion

    #region GetCountryByCountryId Tests
    [Fact]
    public void GetCountryByCountryId_NullCountryId_ReturnsNull()
    {
        // Arrange
        Guid? countryId = null;

        // Act
        CountryResponse? countryResponse = _sut.GetCountryByCountryId(countryId);

        // Assert
        Assert.Null(countryResponse);
    }

    [Fact]
    public void GetCountryByCountryId_InvalidCountryId_ReturnsNull()
    {
        // Arrange
        Guid? countryId = null;

        // Act
        CountryResponse? countryResponse = _sut.GetCountryByCountryId(countryId);

        // Assert
        Assert.Null(countryResponse);
    }

    [Fact]
    public void GetCountryByCountryId_ValidCountryId_ReturnsCountryResponse()
    {
        // Arrange
        var countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Test",
        };
        CountryResponse expectedCountryResponse = _sut.AddCountry(countryAddRequest);
        Guid countryId = expectedCountryResponse.CountryId;

        // Act
        CountryResponse? actualCountryResponse = _sut.GetCountryByCountryId(countryId);

        // Assert
        Assert.NotNull(actualCountryResponse);
        Assert.Equal(expectedCountryResponse, actualCountryResponse);
    }
    #endregion
}
