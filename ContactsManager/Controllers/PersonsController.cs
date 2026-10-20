using Microsoft.AspNetCore.Mvc;
using Services;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ContactsManager.Models;
using System.ComponentModel;

namespace ContactsManager.Controllers;

public class PersonsController : Controller
{
    private readonly ICountryService _countryService;
    private readonly IPersonService _personService;
    private readonly Dictionary<string, string> _searchFields = new Dictionary<string, string>()
    {
        { SearchOptions.ByName.ToString(), "Name" },
        { SearchOptions.ByEmail.ToString(), "Email" },
        { SearchOptions.ByDateOfBirth.ToString(), "Date Of Birth" },
        { SearchOptions.ByGender.ToString(), "Gender" },
        { SearchOptions.ByCountry.ToString(), "Country" },
        { SearchOptions.ByAddress.ToString(), "Address" },
    };

    private readonly List<ValueTuple<string, string>> _sortFields = new List<ValueTuple<string, string>>()
    {
        ( FilterOptions.ByName.ToString(), "Name" ),
        ( FilterOptions.ByEmail.ToString(), "Email" ),
        ( FilterOptions.ByDateOfBirth.ToString(), "Date Of Birth" ),
        ( FilterOptions.ByAge.ToString(), "Age" ),
        ( FilterOptions.ByGender.ToString(), "Gender" ),
        ( FilterOptions.ByCountry.ToString(), "Country" ),
        ( FilterOptions.ByAddress.ToString(), "Address" ),
        ( FilterOptions.ByReceiveNewsLetters.ToString(), "Receives News Letters" )
    };

public PersonsController(
        ICountryService countryService, IPersonService personService)
    {
        _countryService = countryService;
        _personService = personService;
    }

    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index(
        SearchOptions? searchField,
        string? searchString, 
        FilterOptions sortBy = FilterOptions.ByName,
        ListSortDirection sortDirection = ListSortDirection.Ascending)
    {
        

        // if null or empty searchString, then it retrieves all persons
        var allPersons = _personService.GetFilteredPersons(searchString, searchField ?? 0);
        var sortedPersons = _personService.GetSortedPersons(allPersons, sortBy, sortDirection);
        var viewModel = new PersonsViewModel(
            sortedPersons,
            _searchFields,
            _sortFields,
            searchField.ToString() ?? "", searchString, sortBy.ToString(), sortDirection.ToString());

        return View(viewModel);
    }

    [Route("persons/create")]
    [HttpGet]
    public IActionResult Create()
    {
        var countries = _countryService.GetAllCountries();
        return View(countries);
    }
}
