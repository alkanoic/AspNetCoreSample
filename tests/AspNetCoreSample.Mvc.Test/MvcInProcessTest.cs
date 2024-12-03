using System.Diagnostics;
using System.Net;
using System.Text.Json;

using AspNetCoreSample.WebApi.Test;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcInProcessTest : IClassFixture<DbFixture>, IClassFixture<KeycloakFixture>, IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest(DbFixture db, KeycloakFixture keycloak, WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", db.DbConnectionString);
        Environment.SetEnvironmentVariable("KeycloakOptions__Authority", $"{keycloak.BaseAddress}auth/realms/Test");
        Environment.SetEnvironmentVariable("KeycloakOptions__MetadataAddress", $"{keycloak.BaseAddress}realms/Test/.well-known/openid-configuration");
        factory.CreateDefaultClient();
    }

    [Fact]
    [Trait("Category", nameof(MvcInProcessTest))]
    public async Task GetIndexPlaywright()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(PlaywrightSettings.DefaultBrowserTypeLaunchOptions(slowMo: 300));
        await using var context = await browser.NewContextAsync(PlaywrightSettings.DefaultBrowserNewContextOptions());
        PlaywrightSettings.SetDefaultBrowserContext(context);

        var page = await context.NewPageAsync();

        await page.GotoAsync($"{_factory.HostUrl}");
        await page.GetByRole(AriaRole.Link, new() { Name = "Auth" }).ClickAsync();
        await page.GetByLabel("ユーザー名またはメールアドレス").FillAsync("admin");
        await page.GetByLabel("パスワード").FillAsync("admin");
        await page.GetByRole(AriaRole.Button, new() { Name = "ログイン" }).ClickAsync();

        Assert.Contains("Auth Page", await page.TitleAsync());

        await page.GetByRole(AriaRole.Link, new() { Name = "Sample" }).ClickAsync();
        Assert.Contains("AspNetCoreSample.Mvc", await page.TitleAsync());
    }
}
