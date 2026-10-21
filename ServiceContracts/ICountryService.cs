using ServiceContracts.DTOs;

namespace ServiceContracts;

/// <summary>
/// Represents the service contract for Country entities.
/// </summary>
public interface ICountryService
{
    /// <summary>
    /// Adds a new country based on the provided request.
    /// </summary>
    /// <param name="countryAddRequest">The request object containing the details of the country to add.</param>
    /// <returns>A task that results in CountryResponse dto representing the added country.</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

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
