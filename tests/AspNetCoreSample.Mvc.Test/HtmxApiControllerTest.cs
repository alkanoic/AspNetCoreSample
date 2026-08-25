using AspNetCoreSample.Mvc.Controllers;

namespace AspNetCoreSample.Mvc.Test;

public sealed class HtmxApiControllerTest
{
    [Test]
    [Category(nameof(HtmxApiControllerTest))]
    public async Task Fetch_ReturnsValue1AndValue2()
    {
        var controller = new HtmxApiController();

        var result = controller.Fetch("request-value");

        await Assert.That(result.Value1).IsEqualTo("request-value");
        await Assert.That(result.Value2).IsEqualTo("abc");
    }

    [Test]
    [Category(nameof(HtmxApiControllerTest))]
    public async Task FetchPost_ReturnsValue1FromBody()
    {
        var controller = new HtmxApiController();
        var request = new RequestClass { Request = "body-value" };

        var result = controller.FetchPost(request);

        await Assert.That(result.Value1).IsEqualTo("body-value");
        await Assert.That(result.Value2).IsEqualTo("abc");
    }

    [Test]
    [Category(nameof(HtmxApiControllerTest))]
    public async Task FetchPost_WithNullRequest_ReturnsEmptyValue1()
    {
        var controller = new HtmxApiController();
        var request = new RequestClass { Request = null };

        var result = controller.FetchPost(request);

        await Assert.That(result.Value1).IsEqualTo("");
        await Assert.That(result.Value2).IsEqualTo("abc");
    }
}
