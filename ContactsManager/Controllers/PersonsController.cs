using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Controllers;

public class PersonsController : Controller
{
    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }
}
