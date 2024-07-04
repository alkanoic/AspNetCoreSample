using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.WebApi.Services.Keycloak.Admin;
using AspNetCoreSample.WebApi.Services.Keycloak.Token;

namespace AspNetCoreSample.WebApi.Test;

public sealed class KeycloakWebApiTest : IClassFixture<KeycloakFixture>, IDisposable
{
    private readonly KeycloakFixture _keycloakFixture;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private readonly JsonSerializerOptions _jsonTokenSerializerOptions;

    public KeycloakWebApiTest(KeycloakFixture keycloak)
    {
        _keycloakFixture = keycloak;
        _jsonTokenSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    public async void Dispose()
    {
        await _keycloakFixture.DisposeAsync();
    }

    [Fact]
    [Trait("Category", nameof(DbAccessWebApiAuthTest))]
    public async Task PostDbAccessAuth()
    {
        // Given
        var httpClient = new HttpClient() { BaseAddress = new Uri(_keycloakFixture.BaseAddress) };
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = "admin",
            ["password"] = "passwd"
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await httpClient.PostAsync($"realms/master/protocol/openid-connect/token", encodedContent);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, _jsonTokenSerializerOptions);

        // Then
        Assert.NotNull(tokenResponse);
        Assert.NotEmpty(tokenResponse.AccessToken);
        Assert.NotEmpty(tokenResponse.RefreshToken);

        var fetchUserRequest = new HttpRequestMessage(HttpMethod.Get, $"{_keycloakFixture.BaseAddress}admin/realms/Test/users?exact=true&username=admin");
        fetchUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var fetchUserResponse = await httpClient.SendAsync(fetchUserRequest);
        Assert.Equal(HttpStatusCode.OK, fetchUserResponse.StatusCode);

        var fetchUserContent = await fetchUserResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<FetchUserResponse>>(fetchUserContent, _jsonSerializerOptions)?.FirstOrDefault();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Id);
    }
}
