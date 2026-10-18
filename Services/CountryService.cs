using ServiceContracts;
using ServiceContracts.DTOs;
using Entities;
using ServiceContracts.Extensions;

namespace Services;

public class CountryService : ICountryService
{
    /// <summary>
    /// In-memory list to store Country entities.
    /// </summary>
    private readonly List<Country> _countries;

    public CountryService(bool initializeWithSampleData = true)
    {
        _countries = new List<Country>();
        if (initializeWithSampleData)
        {
            _countries.AddRange(new List<Country>
            {
                new Country
                {
                    CountryId = Guid.Parse("9480C358-A557-427E-B430-0419E9522E0F"),
                    CountryName = "Canada"
                },
                new Country
                {
                    CountryId = Guid.Parse("DBF05F1A-FDE3-47D2-AB7E-0DDFF2B662AC"),
                    CountryName = "United States"
                },
                new Country
                {
                    CountryId = Guid.Parse("1CDE0679-9CCE-47FB-B3D2-16CC0CA92C66"),
                    CountryName = "United Kingdom"
                },
                new Country
                {
                    CountryId = Guid.Parse("B7F76A6B-5B0D-4302-9D82-8BFFF29D2F98"),
                    CountryName = "Australia"
                },
                new Country
                {
                    CountryId = Guid.Parse("364BF47F-45AF-4283-B44E-9890C6ED46BB"),
                    CountryName = "Germany"
                },
            });
        }
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

    public CountryResponse? GetCountryById(Guid? countryId)
    {
        if (countryId is null) return null;

        Country? country = _countries.FirstOrDefault(country => country.CountryId == countryId);

        return country?.ToCountryResponse();
    }
}
