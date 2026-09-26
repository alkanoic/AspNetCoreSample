using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.Playwright;

namespace AspNetCoreSample.Spa.Test;

[Collection(nameof(SpaTestFixtures))]
public sealed class SpaWebApiTest
{
    private readonly SpaTestFixture _fixture;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public SpaWebApiTest(SpaTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    [Trait("Category", nameof(SpaWebApiTest))]
    public async ValueTask WebApiIssuesToken()
    {
        using var httpClient = _fixture.CreateWebApiClient();
        var content = new StringContent(JsonSerializer.Serialize(new { userName = "admin", password = "admin" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        using var authResponse = await httpClient.PostAsync(new Uri(new Uri(_fixture.WebApiUrl), "api/Token/Auth"), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);

        using var stream = await authResponse.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        using var token = await JsonDocument.ParseAsync(stream, cancellationToken: TestContext.Current.CancellationToken);
        var accessToken = token.RootElement.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrEmpty(accessToken));

        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(new Uri(_fixture.WebApiUrl), "api/TokenTest/Sample?sample=spa-webapi"));
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        using var sampleResponse = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, sampleResponse.StatusCode);
        Assert.Contains("spa-webapi", await sampleResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    [Trait("Category", nameof(SpaWebApiTest))]
    public async ValueTask SpaLoginViaWebApi()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(PlaywrightSettings.DefaultBrowserTypeLaunchOptions());
        await using var context = await browser.NewContextAsync(PlaywrightSettings.DefaultBrowserNewContextOptions());
        PlaywrightSettings.SetDefaultBrowserContext(context);

        await PlaywrightRetry.RunAsync(async () =>
        {
            var page = await context.NewPageAsync();
            try
            {
                await page.GotoAsync($"{_fixture.SpaUrl}/login", new() { Timeout = 180000 });
                await page.GetByPlaceholder("username").FillAsync("admin");
                await page.GetByPlaceholder("password").FillAsync("admin");
                await page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
                await page.WaitForURLAsync("**/logined", new() { Timeout = 60000 });
                await page.GetByText("PreferredUsername: admin").WaitForAsync(new() { Timeout = 30000 });

                await page.GetByRole(AriaRole.Button, new() { Name = "webapi" }).ClickAsync();
                await page.GetByText("sample", new() { Exact = true }).WaitForAsync(new() { Timeout = 30000 });
            }
            finally
            {
                await page.CloseAsync();
            }
        });
    }
}
