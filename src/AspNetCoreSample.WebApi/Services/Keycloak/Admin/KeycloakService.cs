
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
    ValueTask<FetchUserResponse> FetchUserAsync(FetchUserRequest fetchUserRequest);
    ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest);
    ValueTask UpdateUserAsync(string userId, UpdateUserRequest updateUserRequest);
    ValueTask ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
    ValueTask ResetPasswordByEmailAsync(ResetPasswordByEmailRequest resetPasswordByEmailRequest);
    ValueTask DeleteUserAsync(DeleteUserRequest deleteUserRequest);
    ValueTask<List<FetchRoleResponse>> FetchRolesAsync();
    ValueTask<List<FetchRoleResponse>> FetchUserRoleMappingsAsync(FetchUserRoleMappingsRequest fetchUserRoleMappingsRequest);
    ValueTask AddUserRoleMappingAsync(string userId, List<AddUserRoleMappingsRequest> addUserRoleMappingsRequest);
    ValueTask DeleteUserRoleMappingAsync(string userId, List<DeleteUserRoleMappingsRequest> deleteUserRoleMappingsRequest);
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

    public async ValueTask<FetchUserResponse> FetchUserAsync(FetchUserRequest fetchUserRequest)
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users?exact=true&username={fetchUserRequest.Username}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException($"fetch user fail response detail:{content}");
        }
        var result = JsonSerializer.Deserialize<List<FetchUserResponse>>(content, _jsonSerializerOptions)?.FirstOrDefault();
        if (result is null)
        {
            throw new InvalidDataException($"fetch user fail no content");
        }
        return result;
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

    public async ValueTask UpdateUserAsync(string userId, UpdateUserRequest updateUserRequest)
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(updateUserRequest, updateUserRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"update user fail response detail:{content}");
        }
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

    public async ValueTask ResetPasswordByEmailAsync(ResetPasswordByEmailRequest resetPasswordByEmailRequest)
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{resetPasswordByEmailRequest.UserId}/reset-password-email");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"reset password email fail response detail:{content}");
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

    public async ValueTask<List<FetchRoleResponse>> FetchRolesAsync()
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/roles");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException($"fetch roles fail response detail:{content}");
        }
        var result = JsonSerializer.Deserialize<List<FetchRoleResponse>>(content, _jsonSerializerOptions);
        if (result is null)
        {
            throw new InvalidCastException("fetch roles fail no content");
        }
        return result;
    }

    public async ValueTask<List<FetchRoleResponse>> FetchUserRoleMappingsAsync(FetchUserRoleMappingsRequest fetchUserRoleMappingsRequest)
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{fetchUserRoleMappingsRequest.UserId}/role-mappings/realm");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException($"fetch user roles fail response detail:{content}");
        }
        var result = JsonSerializer.Deserialize<List<FetchRoleResponse>>(content, _jsonSerializerOptions);
        if (result is null)
        {
            throw new InvalidCastException("fetch user roles fail no content");
        }
        return result;
    }

    public async ValueTask AddUserRoleMappingAsync(string userId, List<AddUserRoleMappingsRequest> addUserRoleMappingsRequest)
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/role-mappings/realm");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(addUserRoleMappingsRequest, addUserRoleMappingsRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"add user role mapping fail response detail:{content}");
        }
    }

    public async ValueTask DeleteUserRoleMappingAsync(string userId, List<DeleteUserRoleMappingsRequest> deleteUserRoleMappingsRequest)
    {
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/role-mappings/realm");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(deleteUserRoleMappingsRequest, deleteUserRoleMappingsRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"delete user role mapping fail response detail:{content}");
        }
    }
}
