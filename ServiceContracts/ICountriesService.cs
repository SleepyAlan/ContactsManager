using ServiceContracts.DTOs;

namespace ServiceContracts;

/// <summary>
/// Represents the service contract for Country entities.
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// Adds a new country based on the provided request.
    /// </summary>
    /// <param name="countryAddRequest">The request object containing the details of the country to add.</param>
    /// <returns>The response object representing the added country.</returns>
    CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// Retrieves all countries.
    /// </summary>
    /// <returns>A list of response objects representing all countries.</returns>
    List<CountryResponse> GetAllCountries();
}
