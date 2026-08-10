using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class ComponentController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
