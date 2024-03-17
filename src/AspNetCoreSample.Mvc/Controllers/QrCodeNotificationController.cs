using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class QrCodeNotificationController : Controller
{
    private readonly ILogger<QrCodeNotificationController> _logger;

    public QrCodeNotificationController(ILogger<QrCodeNotificationController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
