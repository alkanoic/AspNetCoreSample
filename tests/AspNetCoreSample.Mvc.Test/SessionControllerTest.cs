using AspNetCoreSample.Mvc.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AspNetCoreSample.Mvc.Test;

public sealed class SessionControllerTest
{
    private static IDistributedCache CreateCache()
    {
        return new MemoryDistributedCache(Microsoft.Extensions.Options.Options.Create(new MemoryDistributedCacheOptions()));
    }

    private static SessionController CreateController(IDistributedCache cache)
    {
        var controller = new SessionController(cache)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Session = new InMemorySession(),
                },
            },
        };
        return controller;
    }

    [Test]
    [Category(nameof(SessionControllerTest))]
    public async Task Index_FirstCall_SetsCacheTime()
    {
        var cache = CreateCache();
        var controller = CreateController(cache);

        var result = await controller.Index();

        await Assert.That(result).IsTypeOf<ViewResult>();
        await Assert.That(controller.ViewData["CacheTime"]).IsNotNull();
        await Assert.That(controller.ViewData["CurrentTime"]).IsNotNull();
    }

    [Test]
    [Category(nameof(SessionControllerTest))]
    public async Task Index_SecondCall_ReturnsSameCacheTime()
    {
        var cache = CreateCache();
        var controller = CreateController(cache);

        await controller.Index();
        var first = controller.ViewData["CacheTime"];

        var secondController = CreateController(cache);
        await secondController.Index();
        var second = secondController.ViewData["CacheTime"];

        await Assert.That(second).IsEqualTo(first);
    }

    [Test]
    [Category(nameof(SessionControllerTest))]
    public async Task Post_StoresSessionAndRedirectsToGet()
    {
        var cache = CreateCache();
        var controller = CreateController(cache);

        var result = controller.Post("test-id");

        var redirect = await Assert.That(result).IsTypeOf<RedirectToActionResult>();
        await Assert.That(redirect.ActionName).IsEqualTo(nameof(SessionController.Get));
        await Assert.That(controller.HttpContext.Session.GetString("TestId")).IsEqualTo("test-id");
    }

    [Test]
    [Category(nameof(SessionControllerTest))]
    public async Task Get_WithSessionData_SetsViewData()
    {
        var cache = CreateCache();
        var controller = CreateController(cache);
        controller.HttpContext.Session.SetString("TestId", "stored-value");

        var result = controller.Get();

        await Assert.That(result).IsTypeOf<ViewResult>();
        await Assert.That(controller.ViewData["SessionData"]).IsEqualTo("stored-value");
    }

    [Test]
    [Category(nameof(SessionControllerTest))]
    public async Task Get_WithoutSessionData_DoesNotSetViewData()
    {
        var cache = CreateCache();
        var controller = CreateController(cache);

        var result = controller.Get();

        await Assert.That(result).IsTypeOf<ViewResult>();
        await Assert.That(controller.ViewData["SessionData"]).IsNull();
    }
}
