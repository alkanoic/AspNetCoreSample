using System.Net;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.WebApi.Services.Keycloak.Token;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class TokenControllerTest : IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public TokenControllerTest()
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
    [Category(nameof(TokenControllerTest))]
    public async Task AuthWithValidCredentialsReturnsToken()
    {
        const string path = "api/Token/Auth";

        var content = new StringContent(JsonSerializer.Serialize(new { userName = "admin", password = "admin" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(stream, JsonSerializerOptions, CancellationToken.None);
        await Assert.That(tokenResponse).IsNotNull();
        await Assert.That(tokenResponse.AccessToken).IsNotEmpty();
        await Assert.That(tokenResponse.RefreshToken).IsNotEmpty();
    }

    [Test]
    [Category(nameof(TokenControllerTest))]
    public async Task AuthWithInvalidCredentialsReturnsBadRequest()
    {
        const string path = "api/Token/Auth";

        var content = new StringContent(JsonSerializer.Serialize(new { userName = "invalid", password = "invalid" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    [Category(nameof(TokenControllerTest))]
    public async Task RefreshTokenWithInvalidTokenReturnsBadRequest()
    {
        const string path = "api/Token/RefreshToken";

        var content = new StringContent(JsonSerializer.Serialize(new { refreshToken = "invalid-refresh-token" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    [Category(nameof(TokenControllerTest))]
    public async Task RevokeTokenWithInvalidTokenReturnsBadRequest()
    {
        const string path = "api/Token/RevokeToken";

        var content = new StringContent(JsonSerializer.Serialize(new { refreshToken = "" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
