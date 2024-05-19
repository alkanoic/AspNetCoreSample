using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class ComponentController : Controller
{
    private readonly ILogger<ComponentController> _logger;

    public ComponentController(ILogger<ComponentController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult My()
    {
        return View();
    }

    public IActionResult Dialog()
    {
        return View();
    }
}
