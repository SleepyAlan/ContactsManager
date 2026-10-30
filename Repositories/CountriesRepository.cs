using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories;

public class CountriesRepository : ICountriesRepository
{
    private readonly ApplicationDbContext _db;

    public CountriesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Country> AddCountry(Country country)
    {
        var addedCountry = _db.Countries.Add(country);
        await _db.SaveChangesAsync();
        return addedCountry.Entity;
    }

    public async Task<bool> CountryExists(Guid countryId)
    {
        return await _db.Countries.AnyAsync(c => c.CountryId == countryId);
    }

    public async Task<bool> CountryExists(string countryName)
    {
        return await _db.Countries.AnyAsync(c => c.CountryName == countryName);
    }

    public async Task<bool> DeleteCountry(Guid countryId)
    {
        var country = await _db.Countries
            .FirstOrDefaultAsync(c => c.CountryId == countryId);
        if (country == null)
        {
            return false;
        }
        _db.Countries.Remove(country);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Country>> GetAllCountries()
    {
        return await _db.Countries.ToListAsync();
    }

    public async Task<Country?> GetCountry(Guid countryId)
    {
        return await _db.Countries
            .FirstOrDefaultAsync(c => c.CountryId == countryId);
    }

    public async Task<Country?> GetCountry(string countryName)
    {
        return await _db.Countries
            .FirstOrDefaultAsync(c => c.CountryName == countryName);
    }

    public async Task<Country> UpdateCountry(Country country)
    {
        var updatedCountry = _db.Countries.Update(country);
        await _db.SaveChangesAsync();
        return updatedCountry.Entity;
    }
}
