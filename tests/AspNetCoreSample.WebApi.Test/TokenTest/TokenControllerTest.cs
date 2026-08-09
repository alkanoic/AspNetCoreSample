using System.Net;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.WebApi.Services.Keycloak.Token;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class TokenControllerTest : IClassFixture<WebApplicationFactoryFixture<Program>>, IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public TokenControllerTest(WebApplicationFactoryFixture<Program> webApplicationFactoryFixture)
    {
        _webApplicationFactoryFixture = webApplicationFactoryFixture;
        _httpClient = _webApplicationFactoryFixture.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(TokenControllerTest))]
    public async Task AuthWithValidCredentialsReturnsToken()
    {
        const string path = "api/Token/Auth";

        var content = new StringContent(JsonSerializer.Serialize(new { userName = "admin", password = "admin" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(stream, JsonSerializerOptions, TestContext.Current.CancellationToken);
        Assert.NotNull(tokenResponse);
        Assert.NotEmpty(tokenResponse.AccessToken);
        Assert.NotEmpty(tokenResponse.RefreshToken);
    }

    [Fact]
    [Trait("Category", nameof(TokenControllerTest))]
    public async Task AuthWithInvalidCredentialsReturnsBadRequest()
    {
        const string path = "api/Token/Auth";

        var content = new StringContent(JsonSerializer.Serialize(new { userName = "invalid", password = "invalid" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(TokenControllerTest))]
    public async Task RefreshTokenWithInvalidTokenReturnsBadRequest()
    {
        const string path = "api/Token/RefreshToken";

        var content = new StringContent(JsonSerializer.Serialize(new { refreshToken = "invalid-refresh-token" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Category", nameof(TokenControllerTest))]
    public async Task RevokeTokenWithInvalidTokenReturnsBadRequest()
    {
        const string path = "api/Token/RevokeToken";

        var content = new StringContent(JsonSerializer.Serialize(new { refreshToken = "" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
