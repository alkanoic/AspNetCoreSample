using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class QrCodeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
