using ServiceContracts;
using ServiceContracts.DTOs;
using Entities;
using ServiceContracts.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class CountryService : ICountryService
{
    private readonly ContactsManagerDbContext _db;

    public CountryService(ContactsManagerDbContext dbContext)
    {
        _db = dbContext;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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

        bool countryExists = await _db.Countries
            .AnyAsync(country => 
                country.CountryName != null &&
                country.CountryName.Equals(countryAddRequest.CountryName, StringComparison.OrdinalIgnoreCase));

        if (countryExists)
        {
            throw new ArgumentException(
                $"Country with name {countryAddRequest.CountryName} already exists.",
                nameof(countryAddRequest.CountryName));
        }

        Country country = countryAddRequest.ToCountry();
        country.CountryId = Guid.NewGuid();

        _db.Countries.Add(country);
        await _db.SaveChangesAsync();

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return await _db.Countries
            .Select(country => country.ToCountryResponse())
            .ToListAsync();
    }

    public async Task<CountryResponse?> GetCountryById(Guid? countryId)
    {
        if (countryId is null) return null;

        Country? country = await _db.Countries
            .FirstOrDefaultAsync(country => country.CountryId == countryId);

        return country?.ToCountryResponse();
    }
}
