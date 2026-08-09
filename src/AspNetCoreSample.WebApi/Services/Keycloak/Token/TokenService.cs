
using System.Text.Json;

using AspNetCoreSample.WebApi.Options;

using Microsoft.Extensions.Options;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Token;

public interface ITokenService
{
    ValueTask<TokenResponse> AuthTokenAsync(TokenRequest tokenRequest, CancellationToken ct = default);
    ValueTask<TokenResponse> RefreshTokenAsync(UpdateTokenRequest updateTokenRequest, CancellationToken ct = default);
    ValueTask RevokeTokenAsync(RevokeTokenRequest revokeTokenRequest, CancellationToken ct = default);
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

    public async ValueTask<TokenResponse> AuthTokenAsync(TokenRequest tokenRequest, CancellationToken ct = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _keycloakOptions.ClientId,
            ["client_secret"] = _keycloakOptions.ClientSecret,
            ["username"] = tokenRequest.UserName,
            ["password"] = tokenRequest.Password
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(_keycloakOptions.TokenEndpoint, encodedContent, ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("authenticate fail response");
        }
        var content = await response.Content.ReadAsStringAsync(ct);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, _jsonSerializerOptions);
        if (tokenResponse == null) throw new InvalidDataException("authenticate fail response");
        return tokenResponse;
    }

    public async ValueTask<TokenResponse> RefreshTokenAsync(UpdateTokenRequest updateTokenRequest, CancellationToken ct = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = _keycloakOptions.ClientId,
            ["client_secret"] = _keycloakOptions.ClientSecret,
            ["refresh_token"] = updateTokenRequest.RefreshToken
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(_keycloakOptions.TokenEndpoint, encodedContent, ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("refresh token fail response");
        }
        var content = await response.Content.ReadAsStringAsync(ct);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, _jsonSerializerOptions);
        if (tokenResponse == null) throw new InvalidDataException("refresh token fail response");
        return tokenResponse;
    }

    public async ValueTask RevokeTokenAsync(RevokeTokenRequest revokeTokenRequest, CancellationToken ct = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = _keycloakOptions.ClientId,
            ["client_secret"] = _keycloakOptions.ClientSecret,
            ["token"] = revokeTokenRequest.RefreshToken
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(_keycloakOptions.RevokeTokenEndpoint, encodedContent, ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("revoke refresh token fail response");
        }
    }
}
