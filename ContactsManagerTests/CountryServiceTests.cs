using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTOs;
using Services;
using Xunit;

namespace Services.Tests;

public class CountryServiceTests
{
    /// <summary>
    /// The System Under Test (SUT) for the CountriesService.
    /// </summary>
    private readonly ICountryService _sut;
    private readonly Mock<ICountriesRepository> _mockCountriesRepository;
    private readonly Fixture _fixture;

    public CountryServiceTests()
    {
        _mockCountriesRepository = new Mock<ICountriesRepository>();

        _fixture = new Fixture();

        // Customize AutoFixture to generate non-empty country names
        _fixture.Customize<CountryAddRequest>(composer =>
            composer
                .With(c => c.CountryName, _fixture.Create<string>())
                .Do(c => c.CountryName = c.CountryName?.Trim() ?? "TestCountry"));

        _fixture.Customize<CountryUpdateRequest>(composer =>
            composer
                .With(c => c.CountryId, Guid.NewGuid())
                .With(c => c.CountryName, _fixture.Create<string>())
                .Do(c => c.CountryName = c.CountryName?.Trim() ?? "TestCountry"));

        _fixture.Customize<Country>(composer =>
            composer
                .With(c => c.CountryId, Guid.NewGuid())
                .With(c => c.CountryName, _fixture.Create<string>())
                .Do(c => c.CountryName = c.CountryName?.Trim() ?? "TestCountry"));

        _sut = new CountryService(_mockCountriesRepository.Object);
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task AddCountry_InvalidCountryName_ThrowsArgumentException(string? countryName)
    {
        // Arrange
        var request = new CountryAddRequest { CountryName = countryName };

        // Act & Assert
        Func<Task> action = async () => await _sut.AddCountry(request);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddCountry_DuplicateCountryName_ThrowsArgumentException()
    {
        // Arrange
        var countryName = _fixture.Create<string>();
        var request = new CountryAddRequest { CountryName = countryName };

        _mockCountriesRepository
            .Setup(repo => repo.CountryExists(countryName))
            .ReturnsAsync(true);

        // Act & Assert
        Func<Task> action = async () => await _sut.AddCountry(request);
        await action.Should().ThrowAsync<ArgumentException>();

        // Verify the repository was called to check for duplicates
        _mockCountriesRepository.Verify(
            repo => repo.CountryExists(countryName),
            Times.Once);
    }

    [Fact]
    public async Task AddCountry_ValidCountryAddRequest_ReturnsCountryResponse()
    {
        // Arrange
        var request = _fixture.Create<CountryAddRequest>();
        var addedCountry = new Country
        {
            CountryId = Guid.NewGuid(),
            CountryName = request.CountryName
        };

        _mockCountriesRepository
            .Setup(repo => repo.CountryExists(request.CountryName!)) // Use null-forgiving operator since we know CountryName is not null here
            .ReturnsAsync(false);

        _mockCountriesRepository
            .Setup(repo => repo.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(addedCountry);

        // Act
        CountryResponse? countryResponse = await _sut.AddCountry(request);

        // Assert
        countryResponse.Should().NotBeNull();
        countryResponse.CountryId.Should().Be(addedCountry.CountryId);
        countryResponse.CountryName.Should().Be(request.CountryName);

        // Verify repository was called correctly
        _mockCountriesRepository.Verify(
            repo => repo.CountryExists(request.CountryName!), // again, null-forgiving operator as we know CountryName is not null
            Times.Once);
        _mockCountriesRepository.Verify(
            repo => repo.AddCountry(It.Is<Country>(c => c.CountryName == request.CountryName)),
            Times.Once);
    }

    #endregion

    #region GetAllCountries Tests

    [Fact]
    public async Task GetAllCountries_NoCountriesAvailable_ReturnsEmptyList()
    {
        // Arrange
        _mockCountriesRepository
            .Setup(repo => repo.GetAllCountries())
            .ReturnsAsync(new List<Country>());

        // Act
        List<CountryResponse> countries = await _sut.GetAllCountries();

        // Assert
        countries.Should().BeEmpty();
        _mockCountriesRepository.Verify(
            repo => repo.GetAllCountries(),
            Times.Once);
    }

    [Fact]
    public async Task GetAllCountries_FewCountriesAvailable_ReturnsAllCountries()
    {
        // Arrange
        var country1 = _fixture.Create<Country>();
        var country2 = _fixture.Create<Country>();
        var countriesList = new List<Country> { country1, country2 };

        _mockCountriesRepository
            .Setup(repo => repo.GetAllCountries())
            .ReturnsAsync(countriesList);

        // Act
        List<CountryResponse> countries = await _sut.GetAllCountries();

        // Assert
        countries.Should().HaveCount(2);
        countries.Should().Contain(c => c.CountryId == country1.CountryId);
        countries.Should().Contain(c => c.CountryId == country2.CountryId);
        _mockCountriesRepository.Verify(
            repo => repo.GetAllCountries(),
            Times.Once);
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

        // Verify repository was not called (short-circuit on null)
        _mockCountriesRepository.Verify(
            repo => repo.GetCountry(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task GetCountryById_NonExistentCountryId_ReturnsNull()
    {
        // Arrange
        var countryId = Guid.NewGuid();
        _mockCountriesRepository
            .Setup(repo => repo.GetCountry(countryId))
            .ReturnsAsync((Country?)null);

        // Act
        CountryResponse? countryResponse = await _sut.GetCountryById(countryId);

        // Assert
        countryResponse.Should().BeNull();
        _mockCountriesRepository.Verify(
            repo => repo.GetCountry(countryId),
            Times.Once);
    }

    [Fact]
    public async Task GetCountryById_ValidCountryId_ReturnsCountryResponse()
    {
        // Arrange
        var country = _fixture.Create<Country>();
        var countryId = country.CountryId;

        _mockCountriesRepository
            .Setup(repo => repo.GetCountry(countryId))
            .ReturnsAsync(country);

        // Act
        CountryResponse? countryResponse = await _sut.GetCountryById(countryId);

        // Assert
        countryResponse.Should().NotBeNull();
        countryResponse.CountryId.Should().Be(country.CountryId);
        countryResponse.CountryName.Should().Be(country.CountryName);

        _mockCountriesRepository.Verify(
            repo => repo.GetCountry(countryId),
            Times.Once);
    }

    #endregion

    #region UpdateCountry Tests

    [Fact]
    public async Task UpdateCountry_NullCountryUpdateRequest_ThrowsArgumentNullException()
    {
        // Arrange
        CountryUpdateRequest? request = null;

        // Act & Assert
        Func<Task> action = async () => await _sut.UpdateCountry(request);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task UpdateCountry_InvalidCountryName_ThrowsArgumentException(string? countryName)
    {
        // Arrange
        var request = new CountryUpdateRequest
        {
            CountryId = Guid.NewGuid(),
            CountryName = countryName
        };

        // Act & Assert
        Func<Task> action = async () => await _sut.UpdateCountry(request);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateCountry_CountryIdDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var request = _fixture.Create<CountryUpdateRequest>();

        _mockCountriesRepository
            .Setup(repo => repo.CountryExists(request.CountryId))
            .ReturnsAsync(false);

        // Act & Assert
        Func<Task> action = async () => await _sut.UpdateCountry(request);
        await action.Should().ThrowAsync<ArgumentException>();

        _mockCountriesRepository.Verify(
            repo => repo.CountryExists(request.CountryId),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCountry_ValidRequest_ReturnsUpdatedCountryResponse()
    {
        // Arrange
        var request = _fixture.Create<CountryUpdateRequest>();
        var updatedCountry = new Country
        {
            CountryId = request.CountryId,
            CountryName = request.CountryName
        };

        _mockCountriesRepository
            .Setup(repo => repo.CountryExists(request.CountryId))
            .ReturnsAsync(true);

        _mockCountriesRepository
            .Setup(repo => repo.UpdateCountry(It.IsAny<Country>()))
            .ReturnsAsync(updatedCountry);

        // Act
        CountryResponse? result = await _sut.UpdateCountry(request);

        // Assert
        result.Should().NotBeNull();
        result.CountryId.Should().Be(request.CountryId);
        result.CountryName.Should().Be(request.CountryName);

        _mockCountriesRepository.Verify(
            repo => repo.CountryExists(request.CountryId),
            Times.Once);
        _mockCountriesRepository.Verify(
            repo => repo.UpdateCountry(It.Is<Country>(c => c.CountryId == request.CountryId && c.CountryName == request.CountryName)),
            Times.Once);
    }

    #endregion
}
