using Microsoft.AspNetCore.Mvc;
using Services;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ContactsManager.Models;

namespace ContactsManager.Controllers;

public class PersonsController : Controller
{

    private readonly ICountryService _countryService;
    private readonly IPersonService _personService;

    public PersonsController(
        ICountryService countryService, IPersonService personService)
    {
        _countryService = countryService;
        _personService = personService;
    }

    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index(string? searchField, string? searchString)
    {
        var searchFields = new Dictionary<string, string>()
        {
            { SearchOptions.ByName.ToString(), "Name" },
            { SearchOptions.ByEmail.ToString(), "Email" },
            { SearchOptions.ByDateOfBirth.ToString(), "Date Of Birth" },
            { SearchOptions.ByGender.ToString(), "Gender" },
            { SearchOptions.ByCountry.ToString(), "Country" },
            { SearchOptions.ByAddress.ToString(), "Address" },
        };

        // if fails, defaults to SearchOptions.ByName
        Enum.TryParse<SearchOptions>(searchField, out var searchOption);

        // if null or empty searchString, then it retrieves all persons
        var allPersons = _personService.GetFilteredPersons(searchString, searchOption);

        var viewModel = new PersonsViewModel(allPersons, searchFields, searchField ?? "", searchString); 

        return View(viewModel);
    }
}
