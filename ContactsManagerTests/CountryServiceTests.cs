using ServiceContracts;
using Entities;
using ServiceContracts.DTOs;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;

namespace Services.Tests;

public class CountryServiceTests
{
    /// <summary>
    /// The System Under Test (SUT) for the CountriesService.
    /// </summary>
    private readonly ICountryService _sut;

    public CountryServiceTests()
    {
        var countries = new List<Country>();


        var dbContext = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        

        _sut = new CountryService(dbContext);
    }

    #region AddCountry Tests
    [Fact]
    public async Task AddCountry_NullCountryAddRequest_ThrowsArgumentNullException()
    {
        // Arrange
        CountryAddRequest? request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _sut.AddCountry(request));
    }

    [Fact]
    public async Task AddCountry_NullCountryName_ThrowsArgumentException()
    {
        // Arrange
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _sut.AddCountry(request));
    }

    [Fact]
    public async Task AddCountry_CountryNameIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _sut.AddCountry(request));
    }

    [Fact]
    public async Task AddCountry_CountryNameIsWhiteSpace_ThrowsArgumentException()
    {
        // Arrange
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = "   "
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _sut.AddCountry(request));
    }

    [Fact]
    public async Task AddCountry_CountryNameIsDuplicate_ThrowsArgumentException()
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
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _sut.AddCountry(request1);
            await _sut.AddCountry(request2);
        });
    }

    [Fact]
    public async Task AddCountry_ValidCountryAddRequest_ReturnsCountryResponse()
    {
        // Arrange
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "Japan"
        };

        // Act
        CountryResponse countryResponse = await _sut.AddCountry(request1);

        // Assert
        Assert.NotNull(countryResponse);
        Assert.True(countryResponse.CountryId != Guid.Empty);
        Assert.Equal(request1.CountryName, countryResponse.CountryName);
    }

    [Fact]
    public async Task AddCountry_ValidCountryAddRequest_VerifyIfValuePersistedViaGetAllCountries()
    {
        // Arrange
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "Japan"
        };

        // Act
        CountryResponse countryResponse = await _sut.AddCountry(request1);
        List<CountryResponse> countries = await _sut.GetAllCountries();

        // Assert
        Assert.Contains(countryResponse, countries);
    }
    #endregion

    #region GetAllCountries Tests
    [Fact]
    public async Task GetAllCountries_NoCountriesAvailable_ReturnsEmptyList()
    {
        // Act
        List<CountryResponse> countries = await _sut.GetAllCountries();

        // Assert
        Assert.Empty(countries);
    }

    [Fact]
    public async Task GetAllCountries_FewCountriesAdded_ReturnsAllCountriesAdded()
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
            await _sut.AddCountry(request1),
            await _sut.AddCountry(request2)
        };

        // Act
        List<CountryResponse> actualCountries = await _sut.GetAllCountries();

        // Assert
        Assert.Equal(2, actualCountries.Count);
        foreach (CountryResponse expectedCountry in expectedCountries)
        {
            Assert.Contains(expectedCountry, actualCountries);
        }
    }
    #endregion

    #region GetCountryById Tests
    [Fact]
    public async Task GetCountryById_NullCountryId_ReturnsNull()
    {
        // Arrange
        Guid? countryId = null;

        // Act
        CountryResponse? countryResponse = await _sut.GetCountryById(countryId);

        // Assert
        Assert.Null(countryResponse);
    }

    [Fact]
    public async Task GetCountryById_InvalidCountryId_ReturnsNull()
    {
        // Arrange
        Guid? countryId = null;

        // Act
        CountryResponse? countryResponse = await _sut.GetCountryById(countryId);

        // Assert
        Assert.Null(countryResponse);
    }

    [Fact]
    public async Task GetCountryById_ValidCountryId_ReturnsCountryResponse()
    {
        // Arrange
        var countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Test",
        };
        CountryResponse expectedCountryResponse = await _sut.AddCountry(countryAddRequest);
        Guid countryId = expectedCountryResponse.CountryId;

        // Act
        CountryResponse? actualCountryResponse = await _sut.GetCountryById(countryId);

        // Assert
        Assert.NotNull(actualCountryResponse);
        Assert.Equal(expectedCountryResponse, actualCountryResponse);
    }
    #endregion
}
