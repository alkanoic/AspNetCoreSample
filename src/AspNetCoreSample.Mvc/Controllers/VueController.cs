using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class VueController : Controller
{
    private readonly ILogger<VueController> _logger;

    public VueController(ILogger<VueController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var vm = new VueIndexViewModel()
        {
            UserName = "UserName",
            Email = "Email",
            Age = 12,
            Birthday = new DateOnly(2000, 10, 1)
        };
        return View(vm);
    }
}
