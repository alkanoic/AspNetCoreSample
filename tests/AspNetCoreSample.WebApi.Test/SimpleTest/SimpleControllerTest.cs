using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class SimpleControllerTest : IClassFixture<WebApplicationFactoryFixture<Program>>, IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public SimpleControllerTest(WebApplicationFactoryFixture<Program> webApplicationFactoryFixture)
    {
        _webApplicationFactoryFixture = webApplicationFactoryFixture;
        _httpClient = _webApplicationFactoryFixture.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(SimpleControllerTest))]
    public async Task GetSimpleReturnsOutput()
    {
        const string path = "api/Simple?input=hello";

        var response = await _httpClient.GetAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        var result = await JsonSerializer.DeserializeAsync<SimpleOutput>(stream, JsonSerializerOptions, TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal("hello", result.Output);
    }

    [Fact]
    [Trait("Category", nameof(SimpleControllerTest))]
    public async Task PostSimpleReturnsOutput()
    {
        const string path = "api/Simple";

        var content = new StringContent(JsonSerializer.Serialize(new { input = "world" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        var result = await JsonSerializer.DeserializeAsync<SimpleOutput>(stream, JsonSerializerOptions, TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal("world", result.Output);
    }

    [Fact]
    [Trait("Category", nameof(SimpleControllerTest))]
    public async Task PostSimpleExceptionReturns500()
    {
        const string path = "api/Simple/Exception";

        var content = new StringContent(JsonSerializer.Serialize(new { input = "test" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(SimpleControllerTest))]
    public async Task GetResourceReturnsLocalizedStrings()
    {
        const string path = "api/Simple/Resource";

        var response = await _httpClient.GetAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
