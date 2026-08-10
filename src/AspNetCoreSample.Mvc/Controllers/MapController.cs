using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class MapController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
