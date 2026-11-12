using Microsoft.AspNetCore.Http;
using ServiceContracts.DTOs;

namespace ServiceContracts.CountryService;

/// <summary>
/// Represents the service contract for adding Country entities.
/// </summary>
public interface ICountryAdderService
{
    /// <summary>
    /// Adds a new country based on the provided request.
    /// </summary>
    /// <param name="countryAddRequest">The request object containing the details of the country to add.</param>
    /// <returns>A task that results in CountryResponse dto representing the added country.</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
    
    /// <summary>
    /// Uploads country/countries from an Excel file.
    /// </summary>
    /// <param name="formFile">Excel file that contains country records to upload.</param>
    /// <returns>A task that results in the number of countries uploaded.</returns>
    Task<int> UploadCountriesFromExcel(IFormFile formFile);
}
