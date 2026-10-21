using Microsoft.AspNetCore.Mvc;
using Services;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ContactsManager.Models;
using System.ComponentModel;

namespace ContactsManager.Controllers;

[Route("[controller]")]
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

    [HttpGet("[action]")]
    [HttpGet("/")]
    public async Task<IActionResult> Index(
        SearchOptions? searchField,
        string? searchString,
        FilterOptions sortBy = FilterOptions.ByName,
        ListSortDirection sortDirection = ListSortDirection.Ascending)
    {
        // if null or empty searchString, then it retrieves all persons
        PersonsViewModel viewModel = await BuildViewModel(searchField, searchString, sortBy, sortDirection);

        return View(viewModel);
    }


    [HttpGet("[action]")]
    public async Task<IActionResult> Create()
    {
        var countries = await _countryService.GetAllCountries();
        return View(countries);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(PersonAddRequest? personAddRequest)
    {
        if (ModelState.IsValid)
        {
            await _personService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }

        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage).ToList();
        ViewBag.Errors = errors;

        var countries = await _countryService.GetAllCountries();
        return View(countries);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Update(Guid? personAddRequest)
    {
        PersonResponse? person;
        try
        {
            person = await _personService.GetPersonById(personAddRequest);

        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Index", "Persons");
        }
        catch (ArgumentException)
        {
            return RedirectToAction("Index", "Persons");
        }

        if (person is null)
        {
            return NotFound();
        }
        return View(person);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Update(PersonUpdateRequest? personUpdateRequest)
    {
        PersonResponse? person;
        try
        {
            person = await _personService.UpdatePerson(personUpdateRequest);
            if (person is null)
            {
                throw new ArgumentException("Person not found");
            }
        }
        catch (Exception e) when (e is ArgumentNullException or ArgumentException)
        {
            return BadRequest();
        }

        return RedirectToAction("Index", "Persons");
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Table(
        SearchOptions? searchField,
        string? searchString,
        FilterOptions sortBy = FilterOptions.ByName,
        ListSortDirection sortDirection = ListSortDirection.Ascending)
    {
        return PartialView("_PersonsTable",
            await BuildViewModel(searchField, searchString, sortBy, sortDirection));
    }


    private async Task<PersonsViewModel> BuildViewModel(SearchOptions? searchField, string? searchString, FilterOptions sortBy, ListSortDirection sortDirection)
    {
        var allPersons = await _personService.GetFilteredPersons(searchString, searchField ?? 0);
        var sortedPersons = _personService.GetSortedPersons(allPersons, sortBy, sortDirection);
        var viewModel = new PersonsViewModel(
            sortedPersons,
            _searchFields,
            _sortFields,
            searchField.ToString() ?? "", searchString, sortBy.ToString(), sortDirection.ToString());
        return viewModel;
    }
}
