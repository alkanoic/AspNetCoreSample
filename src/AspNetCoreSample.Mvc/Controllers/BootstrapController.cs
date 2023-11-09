using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class BootstrapController : Controller
{
    private readonly ILogger<BootstrapController> _logger;

    public BootstrapController(ILogger<BootstrapController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
