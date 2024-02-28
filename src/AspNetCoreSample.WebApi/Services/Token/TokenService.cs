
using AspNetCoreSample.WebApi.Options;

using Microsoft.Extensions.Options;

using System.Text.Json;

namespace AspNetCoreSample.WebApi.Services.Token;

public class TokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;
    public TokenService(HttpClient httpClient, IOptionsSnapshot<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
    }

    public async ValueTask<TokenResponse> GetTokenAsync(TokenRequest tokenRequest)
    {
        var tokenEndpoint = _keycloakOptions.TokenEndpoint;
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _keycloakOptions.ClientId,
            ["client_secret"] = _keycloakOptions.ClientSecret,
            ["username"] = tokenRequest.UserName!,
            ["password"] = tokenRequest.Password!
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(tokenEndpoint, encodedContent);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("authenticate fail response");
        }
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<TokenResponse>(content);
        if (json == null) throw new InvalidDataException("authenticate fail response");
        return json;
    }
}
