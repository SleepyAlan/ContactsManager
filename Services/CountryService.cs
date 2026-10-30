using ServiceContracts;
using ServiceContracts.DTOs;
using Entities;
using ServiceContracts.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Services;

public class CountryService : ICountryService
{
    private readonly ApplicationDbContext _db;

    public CountryService(ApplicationDbContext dbContext)
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
                country.CountryName == countryAddRequest.CountryName);

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
