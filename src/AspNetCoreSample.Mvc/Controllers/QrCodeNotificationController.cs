using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class QrCodeNotificationController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
