using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
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
        Console.WriteLine(model.LightName);
        Console.WriteLine(model.ShadowName);
        return RedirectToAction("Index");
    }

    public class IndexViewModel
    {
        public string? LightName { get; set; }

        public string? ShadowName { get; set; }

        [Display(Name = "メールアドレス")]
        public string? Email { get; set; }

        [Display(Name = "パスワード")]
        public string? Password { get; set; }
    }
}
