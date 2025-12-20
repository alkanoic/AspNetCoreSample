using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

[ClassDataSource<WebApplicationFactoryFixture<Program>>]
public sealed class MvcInProcessTest
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest(WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        factory.CreateDefaultClient();
    }

    [Test]
    [Category(nameof(MvcInProcessTest))]
    public async Task GetIndexPlaywright()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = ["--ignore-certificate-errors"]
        });
        
        await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            Locale = "ja-JP"
        });
        
        context.SetDefaultTimeout(60_000);
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{_factory.HostUrl}");
        await page.GetByRole(AriaRole.Link, new() { Name = "Auth" }).ClickAsync();
        await page.GetByLabel("ユーザー名またはメールアドレス").FillAsync("admin");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "パスワード" }).FillAsync("admin");
        await Task.WhenAll([page.GetByRole(AriaRole.Button, new() { Name = "サインイン" }).ClickAsync(), page.WaitForURLAsync($"{_factory.HostUrl}/Auth")]);

        await Assert.That(await page.TitleAsync()).Contains("Auth Page");

        await page.GetByRole(AriaRole.Link, new() { Name = "Sample" }).ClickAsync();
        await Assert.That(await page.TitleAsync()).Contains("AspNetCoreSample.Mvc");
    }
}
