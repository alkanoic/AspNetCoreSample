
using AspNetCoreSample.WebApi.Options;

using Microsoft.Extensions.Options;

using System.Text.Json;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Token;

public interface ITokenService
{
    ValueTask<TokenResponse> AuthTokenAsync(TokenRequest tokenRequest);
    ValueTask<TokenResponse> RefreshTokenAsync(UpdateTokenRequest updateTokenRequest);
    ValueTask RevokeTokenAsync(RevokeTokenRequest revokeTokenRequest);
}

public class TokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    public TokenService(HttpClient httpClient, IOptionsSnapshot<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    public async ValueTask<TokenResponse> AuthTokenAsync(TokenRequest tokenRequest)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _keycloakOptions.ClientId,
            ["client_secret"] = _keycloakOptions.ClientSecret,
            ["username"] = tokenRequest.UserName!,
            ["password"] = tokenRequest.Password!
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(_keycloakOptions.TokenEndpoint, encodedContent);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("authenticate fail response");
        }
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<TokenResponse>(content, _jsonSerializerOptions);
        if (json == null) throw new InvalidDataException("authenticate fail response");
        return json;
    }

    public async ValueTask<TokenResponse> RefreshTokenAsync(UpdateTokenRequest updateTokenRequest)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = _keycloakOptions.ClientId,
            ["client_secret"] = _keycloakOptions.ClientSecret,
            ["refresh_token"] = updateTokenRequest.RefreshToken
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(_keycloakOptions.TokenEndpoint, encodedContent);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("refresh token fail response");
        }
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<TokenResponse>(content, _jsonSerializerOptions);
        if (json == null) throw new InvalidDataException("refresh token fail response");
        return json;
    }

    public async ValueTask RevokeTokenAsync(RevokeTokenRequest revokeTokenRequest)
    {
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = _keycloakOptions.ClientId,
            ["client_secret"] = _keycloakOptions.ClientSecret,
            ["token"] = revokeTokenRequest.RefreshToken // refresh_token
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(_keycloakOptions.RevokeTokenEndpoint, encodedContent);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("revoke refresh token fail response");
        }
    }
}
