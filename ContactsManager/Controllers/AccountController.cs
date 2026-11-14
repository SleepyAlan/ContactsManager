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
        
        var result = await _signInManager.PasswordSignInAsync(
            userName: loginRequest.Email ?? "",
            password: loginRequest.Password ?? "",
            isPersistent: loginRequest.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("LoginError", "Your account is locked out. Please try again later.");
        }
        else if (result.IsNotAllowed)
        {
            ModelState.AddModelError("LoginError", "You are not allowed to log in. Please contact support.");
        }
        else
        {
            ModelState.AddModelError("LoginError", "Invalid login attempt.");
        }
        return View(loginRequest);
    }
}
