using Microsoft.AspNetCore.Http;
using ServiceContracts.DTOs;

namespace ServiceContracts.CountryService;

/// <summary>
/// Represents the service contract for updating Country entities.
/// </summary>
public interface ICountryUpdaterService
{
    /// <summary>
    /// Updates an existing country based on the provided request.
    /// </summary>
    /// <param name="countryUpdateRequest">The request object containing the details of the country to update.</param>
    /// <returns>A task that results in CountryResponse dto representing the updated country.</returns>
    Task<CountryResponse> UpdateCountry(CountryUpdateRequest? countryUpdateRequest);
}
