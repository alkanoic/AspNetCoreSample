using AspNetCoreSample.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreSample.WebApi.Test;

public sealed class TokenTestControllerTest
{
    private static TokenTestController CreateController()
    {
        var logger = new MockLogger<TokenTestController>();
        return new TokenTestController(logger);
    }
    [Test]
    [Category(nameof(TokenTestControllerTest))]
    public async Task SampleReturnsInput()
    {
        var controller = CreateController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await controller.Sample("hello");

        await Assert.That(result).IsEqualTo("hello");
    }
    [Test]
    [Category(nameof(TokenTestControllerTest))]
    public async Task SampleUserReturnsInput()
    {
        var controller = CreateController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await controller.SampleUser("world");

        await Assert.That(result).IsEqualTo("world");
    }
    [Test]
    [Category(nameof(TokenTestControllerTest))]
    public async Task SampleAdminReturnsInput()
    {
        var controller = CreateController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await controller.SampleAdmin("admin-data");

        await Assert.That(result).IsEqualTo("admin-data");
    }

    private sealed class MockLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }
}
