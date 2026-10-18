using Microsoft.AspNetCore.Mvc;
using Services;
using ServiceContracts;

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
    public IActionResult Index()
    {
        var allPersons = _personService.GetAllPersons();
        return View(allPersons);
    }
}
