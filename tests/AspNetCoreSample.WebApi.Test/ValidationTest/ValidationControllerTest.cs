using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class ValidationControllerTest : IClassFixture<WebApplicationFactoryFixture<Program>>, IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public ValidationControllerTest(WebApplicationFactoryFixture<Program> webApplicationFactoryFixture)
    {
        _webApplicationFactoryFixture = webApplicationFactoryFixture;
        _httpClient = _webApplicationFactoryFixture.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(ValidationControllerTest))]
    public async Task PostValidDataReturnsOk()
    {
        const string path = "api/Validation";

        var data = new
        {
            stringValue = "valid string",
            numberValue = 50,
            dateValue = DateTime.Now
        };
        var content = new StringContent(JsonSerializer.Serialize(data, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(ValidationControllerTest))]
    public async Task PostEmptyStringValueReturnsBadRequest()
    {
        const string path = "api/Validation";

        var data = new
        {
            stringValue = "",
            numberValue = 50,
            dateValue = DateTime.Now
        };
        var content = new StringContent(JsonSerializer.Serialize(data, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(ValidationControllerTest))]
    public async Task PostOutOfRangeNumberReturnsBadRequest()
    {
        const string path = "api/Validation";

        var data = new
        {
            stringValue = "valid",
            numberValue = 200,
            dateValue = DateTime.Now
        };
        var content = new StringContent(JsonSerializer.Serialize(data, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(ValidationControllerTest))]
    public async Task PostOutOfRangeDateReturnsBadRequest()
    {
        const string path = "api/Validation";

        var data = new
        {
            stringValue = "valid",
            numberValue = 50,
            dateValue = DateTime.Now.AddYears(5)
        };
        var content = new StringContent(JsonSerializer.Serialize(data, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
