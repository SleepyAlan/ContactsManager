using ServiceContracts;
using ServiceContracts.DTOs;
using Entities;
using ServiceContracts.Extensions;

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

    /// <summary>
    /// Adds a new country to the collection of countries.
    /// </summary>
    /// <param name="countryAddRequest">The request object containing the details of the country to add.</param>
    /// <returns>The response object representing the added country.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the countryAddRequest is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the country name is null, empty, or already exists. Also thrown when countryAddRequest.CountryName is already exists in the collection.</exception>
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

    public CountryResponse? GetCountryByCountryId(Guid? countryId)
    {
        if (countryId is null) return null;

        Country? country = _countries.FirstOrDefault(country => country.CountryId == countryId);

        return country?.ToCountryResponse();
    }
}
