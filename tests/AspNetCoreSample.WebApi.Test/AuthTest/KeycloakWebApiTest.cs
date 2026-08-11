using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.WebApi.Services.Keycloak.Admin;
using AspNetCoreSample.WebApi.Services.Keycloak.Token;

namespace AspNetCoreSample.WebApi.Test;

public sealed class KeycloakWebApiTest
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private readonly JsonSerializerOptions _jsonTokenSerializerOptions;

    public KeycloakWebApiTest()
    {
        SharedTestContainers.InitializeAsync().GetAwaiter().GetResult();
        _jsonTokenSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    [Test]
    [Category(nameof(DbAccessWebApiAuthTest))]
    public async Task PostDbAccessAuth()
    {
        // Given
        var httpClient = new HttpClient() { BaseAddress = new Uri(SharedTestContainers.KeycloakBaseAddress) };
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = "admin",
            ["password"] = "passwd"
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await httpClient.PostAsync($"realms/master/protocol/openid-connect/token", encodedContent, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync(CancellationToken.None);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, _jsonTokenSerializerOptions);

        // Then
        await Assert.That(tokenResponse).IsNotNull();
        await Assert.That(tokenResponse.AccessToken).IsNotEmpty();
        await Assert.That(tokenResponse.RefreshToken).IsNotEmpty();

        var fetchUserRequest = new HttpRequestMessage(HttpMethod.Get, $"{SharedTestContainers.KeycloakBaseAddress}admin/realms/Test/users?exact=true&username=admin");
        fetchUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var fetchUserResponse = await httpClient.SendAsync(fetchUserRequest, CancellationToken.None);
        await Assert.That(fetchUserResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var fetchUserContent = await fetchUserResponse.Content.ReadAsStringAsync(CancellationToken.None);
        var result = JsonSerializer.Deserialize<List<FetchUserResponse>>(fetchUserContent, _jsonSerializerOptions)?.FirstOrDefault();
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Id).IsNotEmpty();
    }
}
