using ContactsManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ServiceContracts.Extensions;
using Services;
using System.ComponentModel;
using System.Globalization;

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
        PersonsViewModel viewModel = await BuildViewModel(
            searchField, searchString, sortBy, sortDirection);

        return View(viewModel);
    }


    [HttpGet("[action]")]
    public async Task<IActionResult> Create()
    {
        await SetupGenderCountryViewBag();
        return View();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(PersonAddRequest? personAddRequest)
    {
        if (ModelState.IsValid)
        {
            await _personService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }
        await SetupGenderCountryViewBag();
        return View(personAddRequest);
    }


    [HttpGet("[action]/{personId:guid}")]
    public async Task<IActionResult> Update(Guid? personId)
    {
        PersonResponse? person;
        try
        {
            person = await _personService.GetPersonById(personId);

        }
        catch (Exception e) when
            (e is ArgumentNullException or ArgumentException)
        {
            return RedirectToAction("Index", "Persons");
        }

        if (person is null)
        {
            return NotFound();
        }

        var updateRequest = person.ToPersonUpdateRequest();
        await SetupGenderCountryViewBag();
        return View(updateRequest);
    }

    [HttpPost("[action]/{personId:guid}")]
    public async Task<IActionResult> Update(PersonUpdateRequest? personUpdateRequest)
    {
        try
        {
            if (ModelState.IsValid)
            {
                PersonResponse person = await _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index", "Persons");
            }
        }
        catch (Exception e) when (e is ArgumentNullException or ArgumentException)
        {
            return BadRequest();
        }

        await SetupGenderCountryViewBag();
        return View(personUpdateRequest);
    }


    [HttpGet("[action]/{personId:guid}")]
    public async Task<IActionResult> Details(Guid? personId)
    {
        PersonResponse? person;
        try
        {
            person = await _personService.GetPersonById(personId);
            if (person is null)
            {
                throw new ArgumentException("Person not found");
            }
        }
        catch (Exception e) when (e is ArgumentNullException or ArgumentException)
        {
            return NotFound();
        }

        return View(person);
    }

    [HttpPost("[action]/{personId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? personId)
    {
        if (personId is not null)
        {
            await _personService.DeletePerson(personId);
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
        var model = await BuildViewModel(searchField, searchString, sortBy, sortDirection);
        return PartialView("_PersonsTable", model);
    }

    [Route("[action]")]
    public async Task<IActionResult> PersonsPDF(
        SearchOptions? searchField,
        string? searchString,
        FilterOptions sortBy = FilterOptions.ByName,
        ListSortDirection sortDirection = ListSortDirection.Ascending)
    {
        var model = await BuildViewModel(searchField, searchString, sortBy, sortDirection);

        return new ViewAsPdf("ContactsTablePdf", model)
        {
            FileName = "Contacts.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
            PageMargins = new Rotativa.AspNetCore.Options.Margins(12, 10, 12, 10)
        };
    }

    [Route("[action]")]
    public async Task<IActionResult> PersonsCSV(
        SearchOptions? searchField,
        string? searchString,
        FilterOptions sortBy = FilterOptions.ByName,
        ListSortDirection sortDirection = ListSortDirection.Ascending)
    {
        var allPersons = await _personService
            .GetFilteredPersons(searchString, searchField ?? 0);
        var sortedPersons = _personService
            .GetSortedPersons(allPersons, sortBy, sortDirection);
        var memoryStream = await _personService
            .GetPersonsCsvFile(sortedPersons);

        return File(memoryStream, "text/csv", "Contacts.csv");
    }

    [Route("[action]")]
    public async Task<IActionResult> PersonsExcel(
        SearchOptions? searchField,
        string? searchString,
        FilterOptions sortBy = FilterOptions.ByName,
        ListSortDirection sortDirection = ListSortDirection.Ascending)
    {
        var allPersons = await _personService
            .GetFilteredPersons(searchString, searchField ?? 0);
        var sortedPersons = _personService
            .GetSortedPersons(allPersons, sortBy, sortDirection);
        var memoryStream = await _personService
            .GetPersonsExcelFile(sortedPersons);

        return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Contacts.xlsx");
    }

    // Below are private methods that are reusable view models and ViewBag setup
    // for the views above. These methods are not directly accessible
    // via HTTP requests.
    private async Task<PersonsViewModel> BuildViewModel(
        SearchOptions? searchField,
        string? searchString,
        FilterOptions sortBy,
        ListSortDirection sortDirection)
    {
        var allPersons = await _personService.GetFilteredPersons(searchString, searchField ?? 0);
        var sortedPersons = _personService.GetSortedPersons(allPersons, sortBy, sortDirection);
        var viewModel = new PersonsViewModel(
            sortedPersons,
            _searchFields,
            _sortFields,
            searchField.ToString() ?? "",
            searchString,
            sortBy.ToString(),
            sortDirection.ToString());
        return viewModel;
    }

    private async Task SetupGenderCountryViewBag()
    {
        var countries = await _countryService.GetAllCountries();
        var genders = Enum.GetNames(typeof(GenderOptions));
        ViewBag.GenderItems = genders.Select(g => new SelectListItem
        {
            Value = g,
            Text = g
        });
        ViewBag.CountryItems = countries.Select(c => new SelectListItem
        {
            Value = c.CountryId.ToString(),
            Text = c.CountryName
        });
    }
}
