using Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;

namespace ContactsManager.Controllers;

[Route("[controller]")]
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
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

        IdentityResult? result = null;
        // Should not be null due to model validation,
        // but checking to avoid potential null reference
        if (registerDTO.Password is not null) 
        {
            result = await _userManager.CreateAsync(
                user, registerDTO.Password);
        }

        if (result is null)
        {
            _logger.LogError(
                "User creation due to an unexpected error: {RegisterDTO}", registerDTO);
            ModelState.AddModelError("RegisterError", "An unexpected error occurred during registration.");
            return View(registerDTO);
        }

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("RegisterError", error.Description);
            }
            return View(registerDTO);
        }

        // Safeguard: Ensure the "User" role exists before assigning it
        if (!await _roleManager.RoleExistsAsync(
            UserTypeOptions.User.ToString()))
        {
            await _roleManager.CreateAsync(new ApplicationRole(UserTypeOptions.User.ToString()));
        }

        await _userManager.AddToRoleAsync(
            user, UserTypeOptions.User.ToString());

        return RedirectToAction(nameof(Login), "Account");
    }

    [HttpGet("[action]")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(
        LoginDTO loginRequest, string? returnUrl)
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
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
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
            ModelState.AddModelError("LoginError", "Invalid email address or password.");
        }
        return View(loginRequest);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login), "Account");
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> IsEmailAvailable(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return Json(user == null);
    }
}
