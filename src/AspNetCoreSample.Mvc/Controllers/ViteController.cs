using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class ViteController : Controller
{
    private readonly ILogger<ViteController> _logger;

    public ViteController(ILogger<ViteController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
