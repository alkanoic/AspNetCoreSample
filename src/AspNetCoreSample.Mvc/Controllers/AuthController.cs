using System.Diagnostics;

using AspNetCoreSample.Mvc.Models;
using AspNetCoreSample.Util.Logging;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

[Authorize]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    [Logging(LogOnEndArgs = false, LogOnStartArgs = false)]
    public IActionResult Index()
    {
        ViewBag.Claims = HttpContext.User.Claims;
        return View();
    }
}
