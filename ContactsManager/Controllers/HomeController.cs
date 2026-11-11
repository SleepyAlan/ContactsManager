using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Controllers;

[Route("[Controller]")]
public class HomeController : Controller
{
    [Route("[Action]")]
    public IActionResult Error()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is not null)
        {
            ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
        }
        else
        {
            ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
        }

        return View();
    }
}
