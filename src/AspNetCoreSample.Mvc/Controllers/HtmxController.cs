using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class HtmxController : Controller
{
    private readonly ILogger<HtmxController> _logger;

    public HtmxController(ILogger<HtmxController> logger)
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
