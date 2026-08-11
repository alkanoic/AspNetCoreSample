using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class SimpleControllerTest : IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public SimpleControllerTest()
    {
        _webApplicationFactoryFixture = new WebApplicationFactoryFixture<Program>();
        _httpClient = _webApplicationFactoryFixture.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _webApplicationFactoryFixture.Dispose();
    }

    [Test]
    [Category(nameof(SimpleControllerTest))]
    public async Task GetSimpleReturnsOutput()
    {
        const string path = "api/Simple?Input=hello";

        var response = await _httpClient.GetAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        var result = await JsonSerializer.DeserializeAsync<SimpleOutput>(stream, JsonSerializerOptions, CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Output).IsEqualTo("hello");
    }

    [Test]
    [Category(nameof(SimpleControllerTest))]
    public async Task PostSimpleReturnsOutput()
    {
        const string path = "api/Simple";

        var content = new StringContent(JsonSerializer.Serialize(new { input = "world" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        var result = await JsonSerializer.DeserializeAsync<SimpleOutput>(stream, JsonSerializerOptions, CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Output).IsEqualTo("world");
    }

    [Test]
    [Category(nameof(SimpleControllerTest))]
    public async Task PostSimpleExceptionReturns500()
    {
        const string path = "api/Simple/Exception";

        var content = new StringContent(JsonSerializer.Serialize(new { input = "test" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.InternalServerError);
    }

    [Test]
    [Category(nameof(SimpleControllerTest))]
    public async Task GetResourceReturnsLocalizedStrings()
    {
        const string path = "api/Simple/Resource";

        var response = await _httpClient.GetAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }
}
