using ContactsManager.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs;

namespace ContactsManager.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{
    [HttpGet("[action]")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("[action]")]
    public IActionResult Register(RegisterDTO registerDTO)
    {
        if (!ModelState.IsValid)
        {
            return View(registerDTO);
        }

        return RedirectToAction(nameof(AccountController.Login), "Account");
    }

    [HttpGet("[action]")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("[action]")]
    public IActionResult Login(LoginRequestViewModel loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return View(loginRequest);
        }

        return RedirectToAction(nameof(PersonsController.Index), "Person");
    }
}
