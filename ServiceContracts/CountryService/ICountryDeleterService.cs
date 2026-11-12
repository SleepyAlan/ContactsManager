using Microsoft.AspNetCore.Http;
using ServiceContracts.DTOs;

namespace ServiceContracts.CountryService;

/// <summary>
/// Represents the service contract for deleting Country entities.
/// </summary>
public interface ICountryDeleterService
{
    /// <summary>
    /// Deletes an existing country based on the country's unique identifier.
    /// </summary>
    /// <param name="countryId">The unique identifier of the country to delete.</param>
    /// <returns>A task that results in a boolean indicating whether the country was successfully deleted.</returns>
    Task<bool> DeleteCountry(Guid? countryId);
}
