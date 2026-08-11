using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcApiTest : IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _factory;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public MvcApiTest()
    {
        _factory = new WebApplicationFactoryFixture<Program>();
        _httpClient = _factory.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _factory.Dispose();
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetHomeIndexReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetBootstrapReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Bootstrap", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetVueReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Vue", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetVueComponentReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/VueComponent", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetHtmxReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Htmx", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetHtmxApiFetchReturnsJson()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/HtmxApi/Fetch?request=test", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.Content.Headers.ContentType?.ToString()).Contains("application/json");

        var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        var result = await JsonSerializer.DeserializeAsync<JsonElement>(stream, JsonSerializerOptions, CancellationToken.None);
        await Assert.That(result.GetProperty("value1").GetString()).IsEqualTo("test");
        await Assert.That(result.GetProperty("value2").GetString()).IsEqualTo("abc");
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task PostHtmxApiFetchReturnsJson()
    {
        var content = new StringContent(JsonSerializer.Serialize(new { request = "hello" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_factory.HostUrl}/HtmxApi/FetchPost", content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        var result = await JsonSerializer.DeserializeAsync<JsonElement>(stream, JsonSerializerOptions, CancellationToken.None);
        await Assert.That(result.GetProperty("value1").GetString()).IsEqualTo("hello");
        await Assert.That(result.GetProperty("value2").GetString()).IsEqualTo("abc");
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetJQueryIndexReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/JQuery", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetJQueryPartialViewReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/JQuery/PartialViewExample", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task PostJQuerySampleApiReturnsJson()
    {
        var content = new StringContent(JsonSerializer.Serialize(new { text = "test" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_factory.HostUrl}/JQuery/SampleApi", content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        var result = await JsonSerializer.DeserializeAsync<JsonElement>(stream, JsonSerializerOptions, CancellationToken.None);
        await Assert.That(result.GetProperty("text").GetString()).IsEqualTo("test");
        await Assert.That(result.GetProperty("result").GetString() ?? "").IsNotEmpty();
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetSessionIndexReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Session", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetLitReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Lit", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetMapReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Map", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetChatReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Chat", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetComponentReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Component", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetViteReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Vite", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetQrCodeReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/QrCode", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetQrCodeNotificationReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/QrCodeNotification", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetPushReturnsRedirectWhenUnauthenticated()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Push", CancellationToken.None);
        await Assert.That(response.StatusCode).IsNotEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetFluentReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Fluent", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetNameIndexReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Name", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(MvcApiTest))]
    public async Task GetNameCreateReturnsOk()
    {
        var response = await _httpClient.GetAsync($"{_factory.HostUrl}/Name/Create", CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }
}
