using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcInProcessTest2 : PageTest, IAsyncDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest2()
    {
        _factory = new WebApplicationFactoryFixture<Program>();
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
