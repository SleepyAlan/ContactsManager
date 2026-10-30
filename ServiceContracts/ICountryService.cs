using Microsoft.AspNetCore.Http;
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
    /// Updates an existing country based on the provided request.
    /// </summary>
    /// <param name="countryUpdateRequest">The request object containing the details of the country to update.</param>
    /// <returns>A task that results in CountryResponse dto representing the updated country.</returns>
    Task<CountryResponse> UpdateCountry(CountryUpdateRequest? countryUpdateRequest);

    /// <summary>
    /// Deletes an existing country based on the countries unique identifier.
    /// </summary>
    /// <param name="countryId">The unique identifier of the country to delete.</param>
    /// <returns>A task that results in a boolean indicating whether the country was successfully deleted.</returns>
    Task<bool> DeleteCountry(Guid? countryId);
    
    /// <summary>
    /// Uploads country/countries from an Excel file.
    /// </summary>
    /// <param name="formFile">Excel file that contains country records to upload.</param>
    /// <returns>A task that results in the number of countries uploaded.</returns>
    Task<int> UploadCountriesFromExcel(IFormFile formFile);

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
