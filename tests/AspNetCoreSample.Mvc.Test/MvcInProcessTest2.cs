using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcInProcessTest2 : IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest2(WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        factory.CreateDefaultClient();
    }

    [Fact]
    [Trait("Category", nameof(MvcInProcessTest2))]
    public async Task GetIndexPlaywright()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(PlaywrightSettings.DefaultBrowserTypeLaunchOptions());
        var page = await browser.NewPageAsync();
        await page.GotoAsync($"{_factory.HostUrl}");

        Assert.Contains("AspNetCoreSample.Mvc", await page.TitleAsync());
    }
}
