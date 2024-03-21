using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class QrCodeController : Controller
{
    private readonly ILogger<QrCodeController> _logger;

    private readonly IHttpClientFactory _httpClientFactory;

    public QrCodeController(ILogger<QrCodeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

}
