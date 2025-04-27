using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcInProcessTest : IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest(WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        factory.CreateDefaultClient();
    }

    [Fact]
    [Trait("Category", nameof(MvcInProcessTest))]
    public async Task GetIndexPlaywright()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(PlaywrightSettings.DefaultBrowserTypeLaunchOptions());
        await using var context = await browser.NewContextAsync(PlaywrightSettings.DefaultBrowserNewContextOptions());
        PlaywrightSettings.SetDefaultBrowserContext(context);

        var page = await context.NewPageAsync();

        await page.GotoAsync($"{_factory.HostUrl}");
        await page.GetByRole(AriaRole.Link, new() { Name = "Auth" }).ClickAsync();
        await page.GetByLabel("ユーザー名またはメールアドレス").FillAsync("admin");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "パスワード" }).FillAsync("admin");
        await Task.WhenAll([page.GetByRole(AriaRole.Button, new() { Name = "サインイン" }).ClickAsync(), page.WaitForURLAsync($"{_factory.HostUrl}/Auth")]);

        Assert.Contains("Auth Page", await page.TitleAsync());

        await page.GetByRole(AriaRole.Link, new() { Name = "Sample" }).ClickAsync();
        Assert.Contains("AspNetCoreSample.Mvc", await page.TitleAsync());
    }
}
