using System.Globalization;

using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class JQueryController : Controller
{
    private readonly ILogger<JQueryController> _logger;

    public JQueryController(ILogger<JQueryController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async ValueTask<IActionResult> PartialViewExample()
    {
        await Task.Delay(1000);
        var model = "これは部分ビューからのデータです";
        return PartialView("_ExamplePartial", model);
    }

    public SampleResponse SampleApi([FromBody] SampleRequest request)
    {
        return new SampleResponse() { Text = request.Text, Result = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture) };
    }

    public class SampleRequest
    {
        public required string Text { get; set; }
    }

    public class SampleResponse
    {
        public required string Text { get; set; }
        public required string Result { get; set; }
    }
}
