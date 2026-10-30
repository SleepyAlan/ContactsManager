using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Extensions;
using Services.Helpers;

namespace Services;

public class CountryService : ICountryService
{
    private readonly ICountriesRepository _countriesRepository;

    public CountryService(ICountriesRepository countriesRepository)
    {
        _countriesRepository = countriesRepository;
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

        bool countryExists = await _countriesRepository
            .CountryExists(countryAddRequest.CountryName);

        if (countryExists)
        {
            throw new ArgumentException(
                $"Country with name {countryAddRequest.CountryName} already exists.",
                nameof(countryAddRequest.CountryName));
        }

        Country country = countryAddRequest.ToCountry();
        country.CountryId = Guid.NewGuid();

        var addedCountry = await _countriesRepository.AddCountry(country);

        return addedCountry.ToCountryResponse();
    }

    public async Task<CountryResponse> UpdateCountry(CountryUpdateRequest? countryUpdateRequest)
    {
        ArgumentNullException.ThrowIfNull(
            countryUpdateRequest, nameof(countryUpdateRequest));

        ModelValidationHelper.Validate(countryUpdateRequest);

        var countryExists = await _countriesRepository
            .CountryExists(countryUpdateRequest.CountryId);

        if (!countryExists)
        {
            throw new ArgumentException(
                $"Country with CountryId {countryUpdateRequest.CountryId} does not exist.",
                nameof(countryUpdateRequest.CountryId));
        }

        var countryToUpdate = countryUpdateRequest.ToCountry();

        var country = await _countriesRepository.UpdateCountry(countryToUpdate);

        if (country is null)
        {
            throw new Exception(
                $"Country with CountryId {countryUpdateRequest.CountryId} not found. Failed to update.");
        }

        return country.ToCountryResponse();
    }

    public async Task<bool> DeleteCountry(Guid? countryId)
    {
        ArgumentNullException.ThrowIfNull(countryId, nameof(countryId));

        return await _countriesRepository.DeleteCountry(countryId.Value);
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        var countries = await _countriesRepository.GetAllCountries();
        return countries
            .Select(country => country.ToCountryResponse())
            .ToList();
    }

    public async Task<CountryResponse?> GetCountryById(Guid? countryId)
    {
        if (countryId is null) return null;

        Country? country = await _countriesRepository.GetCountry(countryId.Value);

        return country?.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcel(IFormFile formFile)
    {
        var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        using var excelPackage = new ExcelPackage(memoryStream);

        var worksheet = excelPackage.Workbook.Worksheets["Countries"];
        int rowCount = worksheet.Dimension.Rows;
        int countriesUploaded = 0;
        for (int i = 2; i <= rowCount; i++)
        {
            string countryName = worksheet.Cells[i, 1].Text;

            var countryAddRequest = new CountryAddRequest
            {
                CountryName = countryName
            };
            try
            {
                await AddCountry(countryAddRequest);
                countriesUploaded++;
            }
            // Country already exists or invalid, skip to the next one
            catch (ArgumentException) { continue; }
            
        }
        return countriesUploaded;
    }
}
