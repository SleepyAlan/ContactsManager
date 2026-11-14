using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{

    [HttpGet("[action]")]
    public IActionResult Register()
    {
        return View();
    }
}
