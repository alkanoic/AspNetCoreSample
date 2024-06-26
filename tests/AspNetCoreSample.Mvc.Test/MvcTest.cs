using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcTest : IClassFixture<MvcDbFixture>
{
    private readonly MvcDbFixture _mvcFixture;

    public MvcTest(MvcDbFixture mvcFixture)
    {
        _mvcFixture = mvcFixture;
        _mvcFixture.SetBaseAddress();
    }

    [Fact]
    [Trait("Category", nameof(MvcTest))]
    public async Task GetIndexPlaywright()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(PlaywrightSettings.DefaultBrowserTypeLaunchOptions());
        var page = await browser.NewPageAsync();
        await page.GotoAsync(_mvcFixture.BaseAddress!.ToString());

        Assert.Contains("AspNetCoreSample.Mvc", await page.TitleAsync());
    }
}
