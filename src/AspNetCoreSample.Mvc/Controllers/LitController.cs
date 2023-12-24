using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class LitController : Controller
{
    private readonly ILogger<LitController> _logger;

    public LitController(ILogger<LitController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(IndexViewModel model)
    {
        Console.WriteLine(model.Name1);
        return RedirectToAction("Index");
    }

    public class IndexViewModel
    {
        public string? Name1 { get; set; }

        public string? nonComponent { get; set; }
    }
}
