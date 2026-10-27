using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Controllers;

[Route("[controller]")]
public class CountriesController : Controller
{
    [HttpGet("[action]")]
    public IActionResult AddCountry()
    {
        return View();
    }

    [HttpPost("[action]")]
    public IActionResult AddCountry(string? countryName)
    {
        return View();
    }

    [HttpPost("[action]")]
    public IActionResult UploadCountriesFromExcel()
    {
        return View();
    }
}