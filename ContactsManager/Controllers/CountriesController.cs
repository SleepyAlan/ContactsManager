using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTOs;
using Services;

namespace ContactsManager.Controllers;

[Route("[controller]")]
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
    public async Task<IActionResult> AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _countryService.AddCountry(countryAddRequest);
                var countryNameToAdd = countryAddRequest?.CountryName?.Trim() ?? string.Empty;
                countryNameToAdd = countryNameToAdd.Length > 1 ? char.ToUpper(countryNameToAdd[0]) + countryNameToAdd[1..] : countryNameToAdd.ToUpper();
                ViewBag.Message = $"Country '{countryNameToAdd}' has been added successfully.";
            }
            catch (Exception e) when (e is ArgumentException or ArgumentNullException)
            {
                ModelState.AddModelError("CountryName", e.Message);
            }
        }
        return View();
    }

    [HttpPost("[action]")]
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
}