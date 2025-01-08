using System.Diagnostics;
using System.Net;
using System.Text.Json;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcInProcessTest3 : IClassFixture<DbFixture>, IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest3(DbFixture db, WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", db.DbConnectionString);
        factory.CreateDefaultClient();
    }

    [Fact]
    [Trait("Category", nameof(MvcInProcessTest3))]
    public async Task GetIndexPlaywright()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(PlaywrightSettings.DefaultBrowserTypeLaunchOptions());
        var page = await browser.NewPageAsync();
        await page.GotoAsync($"{_factory.HostUrl}/Name");
        await page.GetByRole(AriaRole.Link, new() { Name = "Edit" }).First.ClickAsync();
        await page.GetByLabel("Name1").ClickAsync();
        await page.GetByLabel("Name1").ClickAsync();
        await page.GetByLabel("Name1").FillAsync("太郎123");
        await page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        Assert.Contains("AspNetCoreSample.Mvc", await page.TitleAsync());
    }
}
