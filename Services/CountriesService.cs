using ServiceContracts;
using ServiceContracts.DTOs;
using Entities;

namespace Services;

public class CountriesService : ICountriesService
{
    /// <summary>
    /// In-memory list to store Country entities.
    /// </summary>
    private readonly List<Country> _countries;

    public CountriesService()
    {
        _countries = new List<Country>();
    }

    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        ArgumentNullException.ThrowIfNull(
            countryAddRequest,
            nameof(countryAddRequest));

        if (string.IsNullOrWhiteSpace(countryAddRequest.CountryName))
        {
            throw new ArgumentException(
                "Country name cannot be null or empty.",
                nameof(countryAddRequest.CountryName));
        }

        bool countryExists = _countries
            .Any(country => 
                country.CountryName is not null &&
                country.CountryName.Equals(countryAddRequest.CountryName, StringComparison.OrdinalIgnoreCase));

        if (countryExists)
        {
            throw new ArgumentException(
                $"Country with name {countryAddRequest.CountryName} already exists.",
                nameof(countryAddRequest.CountryName));
        }

        Country country = countryAddRequest.ToCountry();
        country.CountryId = Guid.NewGuid();

        _countries.Add(country);

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        return _countries
            .Select(country => country.ToCountryResponse())
            .ToList();
    }
}
