using AspNetCoreSample.Mvc.Controllers;
using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace AspNetCoreSample.Mvc.Test;

public sealed class HomeControllerTest
{
    private static HomeController CreateController()
    {
        return new HomeController(NullLogger<HomeController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };
    }

    [Test]
    [Category(nameof(HomeControllerTest))]
    public async Task Index_ReturnsView()
    {
        var controller = CreateController();

        var result = controller.Index();

        await Assert.That(result).IsTypeOf<ViewResult>();
    }

    [Test]
    [Category(nameof(HomeControllerTest))]
    public async Task Error_ReturnsViewWithRequestId()
    {
        var controller = CreateController();

        var result = controller.Error();

        var viewResult = await Assert.That(result).IsTypeOf<ViewResult>();
        var model = await Assert.That(viewResult.Model).IsTypeOf<ErrorViewModel>();
        await Assert.That(model.RequestId).IsNotNullOrEmpty();
    }
}
