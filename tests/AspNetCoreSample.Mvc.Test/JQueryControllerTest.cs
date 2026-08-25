using AspNetCoreSample.Mvc.Controllers;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Test;

public sealed class JQueryControllerTest
{
    [Test]
    [Category(nameof(JQueryControllerTest))]
    public async Task Index_ReturnsView()
    {
        var controller = new JQueryController();

        var result = controller.Index();

        await Assert.That(result).IsTypeOf<ViewResult>();
    }

    [Test]
    [Category(nameof(JQueryControllerTest))]
    public async Task PartialViewExample_ReturnsPartialViewWithModel()
    {
        var controller = new JQueryController();

        var result = await controller.PartialViewExample();

        var partialView = await Assert.That(result).IsTypeOf<PartialViewResult>();
        await Assert.That(partialView.ViewName).IsEqualTo("_ExamplePartial");
        await Assert.That(partialView.Model).IsEqualTo("これは部分ビューからのデータです");
    }

    [Test]
    [Category(nameof(JQueryControllerTest))]
    public async Task SampleApi_ReturnsResponseWithText()
    {
        var controller = new JQueryController();
        var request = new JQueryController.SampleRequest { Text = "こんにちは" };

        var result = controller.SampleApi(request);

        await Assert.That(result.Text).IsEqualTo("こんにちは");
        await Assert.That(result.Result).IsNotNullOrEmpty();
    }
}
