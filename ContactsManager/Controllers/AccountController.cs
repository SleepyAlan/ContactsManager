using ContactsManager.Models;
using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs;

namespace ContactsManager.Controllers;

[Route("[controller]")]
public class AccountController(UserManager<ApplicationUser> userManager)
    : Controller
{
    [HttpGet("[action]")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register(RegisterDTO registerDTO)
    {
        if (!ModelState.IsValid)
        {
            return View(registerDTO);
        }
        ApplicationUser user = new ApplicationUser()
        {
            DisplayName = registerDTO.DisplayName,
            UserName = registerDTO.Email,
            Email = registerDTO.Email,
            PhoneNumber = registerDTO.Phone
        };

        IdentityResult result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("RegisterError", error.Description);
            }
            return View(registerDTO);
        }

        return RedirectToAction(nameof(Login), "Account");
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
