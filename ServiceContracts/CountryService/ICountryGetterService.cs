using Microsoft.AspNetCore.Http;
using ServiceContracts.DTOs;

namespace ServiceContracts.CountryService;

/// <summary>
/// Represents the service contract for retrieving Country entities.
/// </summary>
public interface ICountryGetterService
{
    /// <summary>
    /// Retrieves all countries.
    /// </summary>
    /// <returns>A task that results in a list of CountryResponse dto representing all countries.</returns>
    Task<List<CountryResponse>> GetAllCountries();

    /// <summary>
    /// Retrieves a country by its unique identifier.
    /// </summary>
    /// <param name="countryId">The unique identifier of the country.</param>
    /// <returns>A task that results in CountryResponse dto representing the country for that given id. Returns null if the country is not found.</returns>
    Task<CountryResponse?> GetCountryById(Guid? countryId);
}
