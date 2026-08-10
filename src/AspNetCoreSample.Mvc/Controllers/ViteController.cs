using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class ViteController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
