using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

[ClassDataSource<WebApplicationFactoryFixture<Program>>]
public sealed class MvcInProcessTest3
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest3(WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        factory.CreateDefaultClient();
    }

    [Test]
    [Category(nameof(MvcInProcessTest3))]
    public async Task GetIndexPlaywright()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = ["--ignore-certificate-errors", "--disable-web-security"]
        });
        
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        var page = await context.NewPageAsync();
        
        await page.GotoAsync($"{_factory.HostUrl}/Name", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = 30000
        });
        await page.GetByRole(AriaRole.Link, new() { Name = "Edit" }).First.ClickAsync();
        await page.GetByLabel("Name1").ClickAsync();
        await page.GetByLabel("Name1").ClickAsync();
        await page.GetByLabel("Name1").FillAsync("太郎123");
        await page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Assert.That(await page.TitleAsync()).Contains("AspNetCoreSample.Mvc");
    }
}
