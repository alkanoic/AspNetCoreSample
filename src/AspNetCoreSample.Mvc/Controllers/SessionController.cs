using System.Globalization;

using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace AspNetCoreSample.Mvc.Controllers;

public class SessionController : Controller
{
    private readonly IDistributedCache _cache;

    public SessionController(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<IActionResult> Index()
    {
        string? value = await _cache.GetStringAsync("CacheTime");
        if (value == null)
        {
            value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            await _cache.SetStringAsync("CacheTime", value);
        }
        ViewData["CacheTime"] = value;
        ViewData["CurrentTime"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        return View();
    }

    public IActionResult Post(string id)
    {
        HttpContext.Session.SetString("TestId", id);

        return RedirectToAction(nameof(Get));
    }

    public IActionResult Get()
    {
        string? id = HttpContext.Session.GetString("TestId");
        if (!string.IsNullOrWhiteSpace(id))
        {
            ViewData["SessionData"] = id;
        }
        return View(nameof(Index));
    }
}
