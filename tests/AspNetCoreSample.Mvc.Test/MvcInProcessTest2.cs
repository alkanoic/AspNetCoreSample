using Microsoft.Playwright;

using TUnit.Core;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcInProcessTest2 : PageTest, IAsyncDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest2()
    {
        _factory = new WebApplicationFactoryFixture<Program>();
    }

    public override BrowserNewContextOptions ContextOptions(TestContext testContext)
    {
        return new BrowserNewContextOptions { IgnoreHTTPSErrors = true };
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
    }

    [Test]
    [Category(nameof(MvcInProcessTest2))]
    public async Task GetIndexPlaywright()
    {
        _factory.CreateDefaultClient();

        await PlaywrightRetry.RunAsync(async () =>
        {
            await Page.GotoAsync($"{_factory.HostUrl}");

            await Assert.That(await Page.TitleAsync()).Contains("AspNetCoreSample.Mvc");
        });
    }
}
