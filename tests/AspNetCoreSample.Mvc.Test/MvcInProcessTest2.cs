namespace AspNetCoreSample.Mvc.Test;

[ClassDataSource<WebApplicationFactoryFixture<Program>>]
public sealed class MvcInProcessTest2
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest2(WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        factory.CreateDefaultClient();
    }

    [Test]
    [Category(nameof(MvcInProcessTest2))]
    public async Task GetIndexPlaywright()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = ["--ignore-certificate-errors"]
        });

        var page = await browser.NewPageAsync();
        await page.GotoAsync($"{_factory.HostUrl}");

        await Assert.That(await page.TitleAsync()).Contains("AspNetCoreSample.Mvc");
    }
}
