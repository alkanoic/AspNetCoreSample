using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class LitController : Controller
{
    private readonly ILogger<LitController> _logger;

    public LitController(ILogger<LitController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
