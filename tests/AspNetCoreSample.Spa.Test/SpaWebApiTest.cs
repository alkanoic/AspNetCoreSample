using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.Playwright;

namespace AspNetCoreSample.Spa.Test;

[Collection(nameof(SpaTestFixtures))]
public sealed class SpaWebApiTest
{
    private readonly SpaTestFixture _fixture;
    private readonly ITestOutputHelper _output;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public SpaWebApiTest(SpaTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    [Trait("Category", nameof(SpaWebApiTest))]
    public async ValueTask WebApiIssuesToken()
    {
        using var httpClient = SpaTestFixture.CreateWebApiClient();
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

    [Fact(Timeout = 180_000)]
    [Trait("Category", nameof(SpaWebApiTest))]
    public async ValueTask SpaLoginViaWebApi()
    {
        _output.WriteLine("Playwright を初期化します。");
        using var playwright = await Playwright.CreateAsync();
        await using (var browser = await playwright.Chromium.LaunchAsync(PlaywrightSettings.DefaultBrowserTypeLaunchOptions()))
        {
            _output.WriteLine("Chrome を起動しました。");
            await using (var context = await browser.NewContextAsync(PlaywrightSettings.DefaultBrowserNewContextOptions()))
            {
                PlaywrightSettings.SetDefaultBrowserContext(context);
                _output.WriteLine("ブラウザーコンテキストを作成しました。");

                var page = await context.NewPageAsync();
                IResponse? authResponse = null;
                page.Console += (_, message) =>
                {
                    if (message.Type == "error" || message.Text.Contains("login failed", StringComparison.OrdinalIgnoreCase))
                    {
                        _output.WriteLine($"Browser console ({message.Type}): {message.Text}");
                    }
                };
                page.RequestFailed += (_, request) =>
                {
                    if (request.Url.Contains("/api/Token/Auth", StringComparison.OrdinalIgnoreCase))
                    {
                        _output.WriteLine($"Login request failed: {request.Url}: {request.Failure}");
                    }
                };
                page.Response += (_, response) =>
                {
                    if (response.Url.Contains("/api/Token/Auth", StringComparison.OrdinalIgnoreCase))
                    {
                        authResponse = response;
                    }
                };
                try
                {
                    await PlaywrightRetry.RunAsync(async () =>
                    {
                        await page.GotoAsync($"{_fixture.SpaUrl}/login", new()
                        {
                            Timeout = 60000,
                            WaitUntil = WaitUntilState.DOMContentLoaded,
                        });
                    });
                    _output.WriteLine("SPA のログインページを開きました。");
                    await page.GetByPlaceholder("username").FillAsync("admin");
                    await page.GetByPlaceholder("password").FillAsync("admin");
                    await page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
                    _output.WriteLine("WebAPI 経由でログインを要求しました。");
                    try
                    {
                        await page.WaitForURLAsync("**/logined", new()
                        {
                            Timeout = 60000,
                            WaitUntil = WaitUntilState.Commit,
                        });
                    }
                    catch (TimeoutException)
                    {
                        _output.WriteLine($"SPA のログインエラー: {await page.Locator(".text-error").InnerTextAsync()}");
                        if (authResponse != null)
                        {
                            _output.WriteLine($"Login endpoint response: {(int)authResponse.Status} {await authResponse.TextAsync()}");
                        }

                        throw;
                    }

                    await page.GetByText("PreferredUsername: admin").WaitForAsync(new() { Timeout = 30000 });
                    _output.WriteLine("ログイン後の画面を確認しました。");

                    await page.GetByRole(AriaRole.Button, new() { Name = "webapi" }).ClickAsync();
                    await page.GetByText("sample", new() { Exact = true }).WaitForAsync(new() { Timeout = 30000 });
                    _output.WriteLine("認可付き WebAPI 呼び出しを確認しました。");
                }
                finally
                {
                    await page.CloseAsync();
                }
            }
        }

    }
}
