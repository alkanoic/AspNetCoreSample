using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class ValidationControllerTest : IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public ValidationControllerTest()
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
    [Category(nameof(ValidationControllerTest))]
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
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Category(nameof(ValidationControllerTest))]
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
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    [Category(nameof(ValidationControllerTest))]
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
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    [Category(nameof(ValidationControllerTest))]
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
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
