using ServiceContracts;
using Entities;
using ServiceContracts.DTOs;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;
using FluentAssertions;

namespace Services.Tests;

public class CountryServiceTests
{
    /// <summary>
    /// The System Under Test (SUT) for the CountriesService.
    /// </summary>
    private readonly ICountryService _sut;

    public CountryServiceTests()
    {
        var mockDbContext = new DbContextMock<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        var dbContext = mockDbContext.Object;
        var countries = new List<Country>();
        mockDbContext.CreateDbSetMock(db => db.Countries, countries);

        _sut = new CountryService(null);
    }

    #region AddCountry Tests
    [Fact]
    public async Task AddCountry_NullCountryAddRequest_ThrowsArgumentNullException()
    {
        // Arrange
        CountryAddRequest? request = null;

        // Act & Assert
        Func<Task> action = async () => await _sut.AddCountry(request);
        await action.Should().ThrowAsync<ArgumentNullException>();
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
        Func<Task> action = async () => await _sut.AddCountry(request);
        await action.Should().ThrowAsync<ArgumentException>();
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
        Func<Task> action = async () => await _sut.AddCountry(request);
        await action.Should().ThrowAsync<ArgumentException>();
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
        Func<Task> action = async () => await _sut.AddCountry(request);
        await action.Should().ThrowAsync<ArgumentException>();
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
        Func<Task> action = async () =>
        {
            await _sut.AddCountry(request1);
            await _sut.AddCountry(request2);
        };
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddCountry_ValidCountryAddRequest_ReturnsCountryResponse()
    {
        // Arrange
        CountryAddRequest expected = new CountryAddRequest()
        {
            CountryName = "Japan"
        };

        // Act
        CountryResponse countryResponse = await _sut.AddCountry(expected);

        // Assert
        countryResponse.Should().NotBeNull();
        countryResponse.CountryId.Should().NotBe(Guid.Empty);
        countryResponse.CountryName.Should().Be(expected.CountryName);
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
        countries.Should().Contain(countryResponse);
    }
    #endregion

    #region GetAllCountries Tests
    [Fact]
    public async Task GetAllCountries_NoCountriesAvailable_ReturnsEmptyList()
    {
        // Act
        List<CountryResponse> countries = await _sut.GetAllCountries();

        // Assert
        countries.Should().BeEmpty();
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
        List<CountryResponse> countries = await _sut.GetAllCountries();

        // Assert
        countries.Should().HaveCount(2);
        countries.Should().Contain(expectedCountries);
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
        countryResponse.Should().BeNull();
    }

    [Fact]
    public async Task GetCountryById_InvalidCountryId_ReturnsNull()
    {
        // Arrange
        Guid? countryId = null;

        // Act
        CountryResponse? countryResponse = await _sut.GetCountryById(countryId);

        // Assert
        countryResponse.Should().BeNull();
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
        CountryResponse? countryResponse = await _sut.GetCountryById(countryId);

        // Assert
        countryResponse.Should().NotBeNull();
        countryResponse.Should().Be(expectedCountryResponse);
    }
    #endregion
}
