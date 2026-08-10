using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class ChatController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
