using ContactsManager.Filters.ActionFilters;
using ContactsManager.Filters.ExceptionFilters;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTOs;
using System.Globalization;

namespace ContactsManager.Controllers;

[Route("[controller]")]
[TypeFilter(typeof(UnhandledExceptionFilter))]
public class CountriesController : Controller
{
    private readonly ICountryService _countryService;

    public CountriesController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    [HttpGet("[action]")]
    public IActionResult AddCountry()
    {
        return View();
    }

    [HttpPost("[action]")]
    [ValidateAntiForgeryToken]
    [TypeFilter(typeof(TrimWhiteSpaceFromRequestPropertiesFilter))]
    public async Task<IActionResult> AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (countryAddRequest is not null && !string.IsNullOrEmpty(countryAddRequest.CountryName))
                {
                    countryAddRequest.CountryName = ConvertToTitleCase(countryAddRequest.CountryName);
                }
                await _countryService.AddCountry(countryAddRequest);
                ViewBag.Message = $"Country '{countryAddRequest?.CountryName}' has been added successfully.";
            }
            catch (Exception e) when (e is ArgumentException or ArgumentNullException)
            {
                ModelState.AddModelError("CountryName", e.Message);
            }
        }
        return View();
    }

    [HttpPost("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadCountriesFromExcel(IFormFile? excelFile)
    {
        if (
            excelFile is null ||
            excelFile.Length == 0 ||
            (!Path.GetExtension(excelFile.FileName)
                .Equals(".xlsx", StringComparison.OrdinalIgnoreCase)
             &&
            !Path.GetExtension(excelFile.FileName)
                .Equals(".xls", StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("excelFile", "Please select a valid Excel file. 'xlsx' or 'xls' are allowed.");
            return View("AddCountry");
        }
        int count = await _countryService.UploadCountriesFromExcel(excelFile);
        ViewBag.Message = $"{count} countries have been uploaded successfully.";
        return View("AddCountry");
    }

    private string ConvertToTitleCase(string text)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }
}