using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcTest : IClassFixture<MvcFixture>
{
    private readonly MvcFixture _mvcFixture;

    public MvcTest(MvcFixture mvcFixture)
    {
        _mvcFixture = mvcFixture;
        _mvcFixture.SetBaseAddress();
        // Instead of using environment variables to bootstrap our application configuration, we can implement a custom WebApplicationFactory<TEntryPoint>
        // that overrides the ConfigureWebHost(IWebHostBuilder) method to add a WeatherDataContext to the service collection.
        // Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://+");
        // Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", "certificate.crt");
        // Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password", "password");
        // Environment.SetEnvironmentVariable("ConnectionStrings__Default", MvcFixture.DbConnectionString);
        // _webApplicationFactory = new WebApplicationFactory<Program>();
        // _serviceScope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        // _httpClient = _webApplicationFactory.CreateClient();
    }

    [Fact]
    [Trait("Category", nameof(MvcTest))]
    public async Task GetIndexPlaywright()
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(_mvcFixture.BaseAddress!.ToString());

            // Expect a title "to contain" a substring.
            Assert.Contains("Playwright", await page.TitleAsync());
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
