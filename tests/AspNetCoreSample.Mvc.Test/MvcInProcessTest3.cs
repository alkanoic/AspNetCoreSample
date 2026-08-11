using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcInProcessTest3 : PageTest, IAsyncDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest3()
    {
        _factory = new WebApplicationFactoryFixture<Program>();
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
    }

    [Test]
    [Category(nameof(MvcInProcessTest3))]
    public async Task GetIndexPlaywright()
    {
        _factory.CreateDefaultClient();

        await PlaywrightRetry.RunAsync(async () =>
        {
            await Page.GotoAsync($"{_factory.HostUrl}/Name");
            await Page.GetByRole(AriaRole.Link, new() { Name = "Edit" }).First.ClickAsync();
            await Page.GetByLabel("Name1").ClickAsync();
            await Page.GetByLabel("Name1").ClickAsync();
            await Page.GetByLabel("Name1").FillAsync("太郎123");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

            await Assert.That(await Page.TitleAsync()).Contains("AspNetCoreSample.Mvc");
        });

        await SharedTestContainers.ResetNameTableAsync();
    }
}
