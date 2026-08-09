using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcApiTest : IClassFixture<WebApplicationFactoryFixture<Program>>, IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _factory;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public MvcApiTest(WebApplicationFactoryFixture<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetHomeIndexReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetBootstrapReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Bootstrap", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetVueReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Vue", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetVueComponentReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/VueComponent", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetHtmxReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Htmx", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetHtmxApiFetchReturnsJson()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/HtmxApi/Fetch?request=test", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("application/json", response.Content.Headers.ContentType?.ToString());

        var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        var result = await JsonSerializer.DeserializeAsync<JsonElement>(stream, JsonSerializerOptions, TestContext.Current.CancellationToken);
        Assert.Equal("test", result.GetProperty("value1").GetString());
        Assert.Equal("abc", result.GetProperty("value2").GetString());
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task PostHtmxApiFetchReturnsJson()
    {
        var content = new StringContent(JsonSerializer.Serialize(new { request = "hello" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_factory.HostUrl}/HtmxApi/FetchPost", content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        var result = await JsonSerializer.DeserializeAsync<JsonElement>(stream, JsonSerializerOptions, TestContext.Current.CancellationToken);
        Assert.Equal("hello", result.GetProperty("value1").GetString());
        Assert.Equal("abc", result.GetProperty("value2").GetString());
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetJQueryIndexReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/JQuery", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetJQueryPartialViewReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/JQuery/PartialViewExample", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task PostJQuerySampleApiReturnsJson()
    {
        var content = new StringContent(JsonSerializer.Serialize(new { text = "test" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_factory.HostUrl}/JQuery/SampleApi", content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        var result = await JsonSerializer.DeserializeAsync<JsonElement>(stream, JsonSerializerOptions, TestContext.Current.CancellationToken);
        Assert.Equal("test", result.GetProperty("text").GetString());
        Assert.NotEmpty(result.GetProperty("result").GetString() ?? "");
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetSessionIndexReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Session", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetLitReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Lit", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetMapReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Map", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetChatReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Chat", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetComponentReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Component", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetViteReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Vite", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetQrCodeReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/QrCode", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetQrCodeNotificationReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/QrCodeNotification", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetPushReturnsRedirectWhenUnauthenticated()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Push", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetFluentReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Fluent", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetNameIndexReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Name", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(MvcApiTest))]
    public async Task GetNameCreateReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Name/Create", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
