using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AspNetCoreSample.Mvc.Models;

namespace AspNetCoreSample.Mvc.Controllers;

public class PushController : Controller
{
    private readonly ILogger<PushController> _logger;

    private readonly Options.VapidOption _vapidOption;

    public PushController(ILogger<PushController> logger, Options.VapidOption vapidOptions)
    {
        _logger = logger;
        _vapidOption = vapidOptions;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Register()
    {
        var vm = new Models.PushRegisterViewModel();
        vm.PublicKey = _vapidOption.PublicKey;
        return View(vm);
    }

    public IActionResult SendPush()
    {
        return View();
    }

    public static Models.SubscribeViewModel? _subscribeViewModel;

    [HttpPost]
    public void Subscribe([FromBody] Models.SubscribeViewModel subscribeViewModel)
    {
        _subscribeViewModel = subscribeViewModel;
    }

    public async Task Trigger()
    {

        var client = new WebPush.WebPushClient();
        var subscription = new WebPush.PushSubscription
        {
            Auth = _subscribeViewModel?.Keys?.Auth,
            P256DH = _subscribeViewModel?.Keys?.P256dh,
            Endpoint = _subscribeViewModel?.Endpoint
        };
        var vapid = new WebPush.VapidDetails();
        vapid.Subject = "mailto:test@example.com";
        vapid.PublicKey = _vapidOption.PublicKey;
        vapid.PrivateKey = _vapidOption.PrivateKey;
        var payload = System.Text.Json.JsonSerializer.Serialize(new { title = "title", msg = "message", icon = "/images/icon.jpg" });
        try
        {
            await client.SendNotificationAsync(subscription, payload, vapid);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

}
