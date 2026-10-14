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
        CountryAddRequest? request = null;

        Assert.Throws<ArgumentNullException>(
            () => _sut.AddCountry(request));
    }

    [Fact]
    public void AddCountry_WhenCountryNameIsNull_ThrowsArgumentException()
    {
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = null
        };

        Assert.Throws<ArgumentException>(
            () => _sut.AddCountry(request));
    }

    [Fact]
    public void AddCountry_WhenCountryNameIsEmpty_ThrowsArgumentException()
    {
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = ""
        };

        Assert.Throws<ArgumentException>(
            () => _sut.AddCountry(request));
    }

    [Fact]
    public void AddCountry_WhenCountryNameIsWhiteSpace_ThrowsArgumentException()
    {
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = "   "
        };

        Assert.Throws<ArgumentException>(
            () => _sut.AddCountry(request));
    }

    [Fact]
    public void AddCountry_WhenCountryNameIsDuplicate_ThrowsArgumentException()
    {
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "USA"
        };
        CountryAddRequest request2 = new CountryAddRequest()
        {
            CountryName = "USA"
        };


        Assert.Throws<ArgumentException>(() =>
        {
            _sut.AddCountry(request1);
            _sut.AddCountry(request2);
        });
    }

    [Fact]
    public void AddCountry_WhenCountryNameSuppliedIsProper_CountryShouldBeInserted()
    {
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "Japan"
        };

        CountryResponse countryResponse = _sut.AddCountry(request1);

        Assert.True(countryResponse.CountryId != Guid.Empty);
    }
    #endregion

}
