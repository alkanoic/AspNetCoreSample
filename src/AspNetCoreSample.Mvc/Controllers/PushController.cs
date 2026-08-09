using System.Diagnostics;

using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCoreSample.Mvc.Controllers;

[Authorize]
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public void Subscribe([FromBody] Models.SubscribeViewModel subscribeViewModel)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        PushSubscriptionStore.Set(userId, subscribeViewModel);
    }

    public async Task Trigger()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        var subscribeViewModel = PushSubscriptionStore.Get(userId);
        if (subscribeViewModel == null)
        {
            _logger.LogWarning("Push subscription not found for user {UserId}", userId);
            return;
        }

        var client = new WebPush.WebPushClient();
        var subscription = new WebPush.PushSubscription
        {
            Auth = subscribeViewModel.Keys?.Auth,
            P256DH = subscribeViewModel.Keys?.P256dh,
            Endpoint = subscribeViewModel.Endpoint
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
            _logger.LogError(ex, "Push notification failed");
        }
    }

}
