using ServiceContracts;
using Entities;
using ServiceContracts.DTOs;
using Services;

namespace ContactsManagerTests;

public class CountriesServiceTests
{
    /// <summary>
    /// The System Under Test (SUT) for the CountriesService.
    /// </summary>
    private readonly ICountriesService _sut;

    public CountriesServiceTests()
    {
        _sut = new CountriesService();
    }

    #region AddCountry Tests
    [Fact]
    public void AddCountry_WhenCountryAddRequestIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        CountryAddRequest? request = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _sut.AddCountry(request));
    }

    [Fact]
    public void AddCountry_WhenCountryNameIsNull_ThrowsArgumentException()
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
    public void AddCountry_WhenCountryNameIsEmpty_ThrowsArgumentException()
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
    public void AddCountry_WhenCountryNameIsWhiteSpace_ThrowsArgumentException()
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
    public void AddCountry_WhenCountryNameIsDuplicate_ThrowsArgumentException()
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
    public void AddCountry_WhenProperCountryAddRequest_ValueShouldBeInserted()
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
    public void AddCountry_WhenProperCountryAddRequest_VerifyIfValuePersisted()
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
    public void GetAllCountries_WhenNoCountriesAdded_ReturnsEmptyList()
    {
        // Act
        List<CountryResponse> countries = _sut.GetAllCountries();

        // Assert
        Assert.Empty(countries);
    }

    [Fact]
    public void GetAllCountries_WhenFewCountriesAdded_ReturnsAllCountriesAdded()
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
        List<CountryResponse> expectedCountries = new ()
        {
            _sut.AddCountry(request1),
            _sut.AddCountry(request2)
        };

        // Act
        List<CountryResponse> actualCountries = _sut.GetAllCountries();

        // Assert
        Assert.Equal(2, actualCountries.Count);
        foreach(CountryResponse expectedCountry in expectedCountries)
        {
            Assert.Contains(expectedCountry, actualCountries);
        }
    }
    #endregion
}
