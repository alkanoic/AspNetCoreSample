
using AspNetCoreSample.WebApi.Options;
using AspNetCoreSample.WebApi.Services.Keycloak.Token;

using Microsoft.Extensions.Options;

using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public interface IKeycloakService
{
    ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest);
    ValueTask ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
    ValueTask DeleteUserAsync(DeleteUserRequest deleteUserRequest);
}

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;
    private readonly JsonSerializerOptions _jsonTokenSerializerOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public KeycloakService(HttpClient httpClient, IOptionsSnapshot<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
        _httpClient.BaseAddress = new Uri(_keycloakOptions.AdminBaseAddress);
        _jsonTokenSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    private async ValueTask<TokenResponse> AdminAccessToken()
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = _keycloakOptions.AdminUserName,
            ["password"] = _keycloakOptions.AdminPassword
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(_keycloakOptions.AdminTokenEndpoint, encodedContent);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("authenticate fail response");
        }
        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, _jsonTokenSerializerOptions);
        if (tokenResponse == null) throw new InvalidDataException("authenticate fail response");
        return tokenResponse;
    }

    public async ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest)
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(createUserRequest, createUserRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"create user fail response detail:{content}");
        }
        var segments = response.Headers.Location?.LocalPath.Split('/');
        return new CreateUserResponse() { Id = segments?[segments.Length - 1] ?? "" };
    }

    public async ValueTask ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{changePasswordRequest.UserId}/reset-password");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(changePasswordRequest.Credential, changePasswordRequest.Credential.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"change password fail response detail:{content}");
        }
    }

    public async ValueTask DeleteUserAsync(DeleteUserRequest deleteUserRequest)
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{deleteUserRequest.UserId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"delete user fail response detail:{content}");
        }
    }
}
