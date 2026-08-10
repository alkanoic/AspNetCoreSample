using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class VueComponentController : Controller
{
    public IActionResult Index()
    {
        return View(VueIndexViewModel.CreateSample());
    }
}
