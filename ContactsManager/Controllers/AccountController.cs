using ContactsManager.Models;
using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs;

namespace ContactsManager.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

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
        var user = new ApplicationUser()
        {
            DisplayName = registerDTO.DisplayName,
            UserName = registerDTO.Email,
            Email = registerDTO.Email,
            PhoneNumber = registerDTO.Phone
        };

        //TODO: password null and matching confirm password validation
        IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

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
    public async Task<IActionResult> Login(LoginRequestViewModel loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return View(loginRequest);
        }
        
        ApplicationUser user = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null)
        {
            ModelState.AddModelError("LoginError", "Invalid login attempt.");
            return View(loginRequest);
        }

        await _signInManager.SignInAsync(user, isPersistent: true);
        return RedirectToAction(nameof(PersonsController.Index), "Person");
    }
}
