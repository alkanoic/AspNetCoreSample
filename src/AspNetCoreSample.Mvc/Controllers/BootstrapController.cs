using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class BootstrapController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
