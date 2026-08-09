
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.WebApi.Models.Keycloak;
using AspNetCoreSample.WebApi.Options;
using AspNetCoreSample.WebApi.Services.Keycloak.Token;

using Microsoft.Extensions.Options;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public interface IKeycloakService
{
    ValueTask<FetchUserResponse> FetchUserAsync(FetchUserRequest fetchUserRequest, CancellationToken ct = default);
    ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken ct = default);
    ValueTask UpdateUserAsync(string userId, UpdateUserRequest updateUserRequest, CancellationToken ct = default);
    ValueTask UpdateUserByUsernameAsync(string username, UpdateUserRequest updateUserRequest, CancellationToken ct = default);
    ValueTask ChangePasswordAsync(string userId, ChangePasswordRequest changePasswordRequest, CancellationToken ct = default);
    ValueTask ChangePasswordByUsernameAsync(string username, ChangePasswordRequest changePasswordRequest, CancellationToken ct = default);
    ValueTask ResetPasswordByEmailAsync(string userId, ResetPasswordByEmailRequest resetPasswordByEmailRequest, CancellationToken ct = default);
    ValueTask ResetPasswordByEmailByUsernameAsync(string username, ResetPasswordByEmailRequest resetPasswordByEmailRequest, CancellationToken ct = default);
    ValueTask DeleteUserAsync(string userId, DeleteUserRequest deleteUserRequest, CancellationToken ct = default);
    ValueTask DeleteUserByUsernameAsync(string username, DeleteUserRequest deleteUserRequest, CancellationToken ct = default);
    ValueTask<List<FetchRoleResponse>> FetchRolesAsync(CancellationToken ct = default);
    ValueTask<List<FetchRoleResponse>> FetchUserRoleMappingsAsync(FetchUserRoleMappingsRequest fetchUserRoleMappingsRequest, CancellationToken ct = default);
    ValueTask AddUserRoleMappingAsync(string userId, List<AddUserRoleMappingsRequest> addUserRoleMappingsRequest, CancellationToken ct = default);
    ValueTask DeleteUserRoleMappingAsync(string userId, List<DeleteUserRoleMappingsRequest> deleteUserRoleMappingsRequest, CancellationToken ct = default);
    ValueTask<List<KeycloakClientResponse>> FetchClientsAsync(FetchClientsRequest fetchClientsRequest, CancellationToken ct = default);
    ValueTask<KeycloakClientResponse> FetchClientAsync(FetchClientRequest fetchClientRequest, CancellationToken ct = default);
    ValueTask<List<FetchRoleResponse>> FetchClientRolesAsync(FetchClientRolesRequest fetchClientRolesRequest, CancellationToken ct = default);
    ValueTask<List<FetchRoleResponse>> FetchUserClientRolesAsync(FetchUserClientRolesRequest fetchUserClientRolesRequest, CancellationToken ct = default);
    ValueTask AddUserClientRoleMappingAsync(string userId, string clientUuid, List<AddUserRoleMappingsRequest> addUserClientRoleMappingRequest, CancellationToken ct = default);
    ValueTask DeleteUserClientRoleMappingAsync(string userId, string clientUuid, List<DeleteUserRoleMappingsRequest> deleteUserClientRoleMappingRequest, CancellationToken ct = default);
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

    /// <summary>
    /// Optionsのユーザー情報で代理アクセスする場合
    /// 引数にAccessTokenが与えられた場合は使用しない
    /// </summary>
    private async ValueTask<TokenResponse> AdminAccessToken(CancellationToken ct = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = _keycloakOptions.AdminUserName,
            ["password"] = _keycloakOptions.AdminPassword
        };

        var encodedContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync(_keycloakOptions.AdminTokenEndpoint, encodedContent, ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("authenticate fail response");
        }
        var content = await response.Content.ReadAsStringAsync(ct);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, _jsonTokenSerializerOptions);
        if (tokenResponse == null) throw new InvalidDataException("authenticate fail response");
        return tokenResponse;
    }

    /// <summary>
    /// ユーザー名からユーザー情報を取得する
    /// ユーザー名は完全一致で取得
    /// </summary>
    public async ValueTask<FetchUserResponse> FetchUserAsync(FetchUserRequest fetchUserRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(fetchUserRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users?exact=true&username={Uri.EscapeDataString(fetchUserRequest.Username)}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchUserRequest.AccessToken);

        var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
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

    /// <summary>
    /// ユーザーを作成する
    /// </summary>
    public async ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(createUserRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", createUserRequest.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(createUserRequest, createUserRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidDataException($"create user fail response detail:{content}");
        }
        var segments = response.Headers.Location?.LocalPath.Split('/');
        return new CreateUserResponse() { Id = segments?[segments.Length - 1] ?? "" };
    }

    /// <summary>
    /// ユーザー情報を更新する
    /// </summary>
    public async ValueTask UpdateUserAsync(string userId, UpdateUserRequest updateUserRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(updateUserRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", updateUserRequest.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(updateUserRequest, updateUserRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidDataException($"update user fail response detail:{content}");
        }
    }

    /// <summary>
    /// ユーザー名でユーザー情報を更新
    /// </summary>
    public async ValueTask UpdateUserByUsernameAsync(string username, UpdateUserRequest updateUserRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(updateUserRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var fetchUserResponse = await FetchUserAsync(new FetchUserRequest() { Username = username, AccessToken = updateUserRequest.AccessToken }, ct);
        await UpdateUserAsync(fetchUserResponse.Id, updateUserRequest, ct);
    }

    /// <summary>
    /// ユーザーのパスワード変更
    /// </summary>
    public async ValueTask ChangePasswordAsync(string userId, ChangePasswordRequest changePasswordRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(changePasswordRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/reset-password");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", changePasswordRequest.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(changePasswordRequest.Credential, changePasswordRequest.Credential.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidDataException($"change password fail response detail:{content}");
        }
    }

    /// <summary>
    /// ユーザー名でユーザーのパスワード変更
    /// </summary>
    public async ValueTask ChangePasswordByUsernameAsync(string username, ChangePasswordRequest changePasswordRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(changePasswordRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var fetchUserResponse = await FetchUserAsync(new FetchUserRequest() { AccessToken = changePasswordRequest.AccessToken, Username = username }, ct);
        await ChangePasswordAsync(fetchUserResponse.Id, changePasswordRequest, ct);
    }

    public async ValueTask ResetPasswordByEmailAsync(string userId, ResetPasswordByEmailRequest resetPasswordByEmailRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(resetPasswordByEmailRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/reset-password-email");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", resetPasswordByEmailRequest.AccessToken);

        var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidDataException($"reset password email fail response detail:{content}");
        }
    }

    public async ValueTask ResetPasswordByEmailByUsernameAsync(string username, ResetPasswordByEmailRequest resetPasswordByEmailRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(resetPasswordByEmailRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var fetchUserResponse = await FetchUserAsync(new FetchUserRequest() { Username = username, AccessToken = resetPasswordByEmailRequest.AccessToken }, ct);
        await ResetPasswordByEmailAsync(fetchUserResponse.Id, resetPasswordByEmailRequest, ct);
    }

    public async ValueTask DeleteUserAsync(string userId, DeleteUserRequest deleteUserRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(deleteUserRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", deleteUserRequest.AccessToken);

        var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidDataException($"delete user fail response detail:{content}");
        }
    }

    public async ValueTask DeleteUserByUsernameAsync(string username, DeleteUserRequest deleteUserRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(deleteUserRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var fetchUserResponse = await FetchUserAsync(new FetchUserRequest() { Username = username, AccessToken = deleteUserRequest.AccessToken }, ct);
        await DeleteUserAsync(fetchUserResponse.Id, deleteUserRequest, ct);
    }

    public async ValueTask<List<FetchRoleResponse>> FetchRolesAsync(CancellationToken ct = default)
    {
        var tokenResponse = await AdminAccessToken(ct);
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/roles");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
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

    public async ValueTask<List<FetchRoleResponse>> FetchUserRoleMappingsAsync(FetchUserRoleMappingsRequest fetchUserRoleMappingsRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(fetchUserRoleMappingsRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{fetchUserRoleMappingsRequest.UserId}/role-mappings/realm");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchUserRoleMappingsRequest.AccessToken);

        var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
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

    public async ValueTask AddUserRoleMappingAsync(string userId, List<AddUserRoleMappingsRequest> addUserRoleMappingsRequest, CancellationToken ct = default)
    {
        var tokenResponse = await AdminAccessToken(ct);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/role-mappings/realm");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(addUserRoleMappingsRequest, addUserRoleMappingsRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidDataException($"add user role mapping fail response detail:{content}");
        }
    }

    public async ValueTask DeleteUserRoleMappingAsync(string userId, List<DeleteUserRoleMappingsRequest> deleteUserRoleMappingsRequest, CancellationToken ct = default)
    {
        var tokenResponse = await AdminAccessToken(ct);
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/role-mappings/realm");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(deleteUserRoleMappingsRequest, deleteUserRoleMappingsRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidDataException($"delete user role mapping fail response detail:{content}");
        }
    }

    public async ValueTask<List<KeycloakClientResponse>> FetchClientsAsync(FetchClientsRequest fetchClientsRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(fetchClientsRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/clients");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchClientsRequest.AccessToken);

        var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException($"fetch clients fail response detail:{content}");
        }
        var results = JsonSerializer.Deserialize<List<KeycloakClientResponse>>(content, _jsonSerializerOptions);
        if (results is null)
        {
            throw new InvalidCastException("fetch keycloak client fail no content");
        }
        return results;
    }

    public async ValueTask<KeycloakClientResponse> FetchClientAsync(FetchClientRequest fetchClientRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(fetchClientRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/clients?clientId={Uri.EscapeDataString(fetchClientRequest.ClientId)}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchClientRequest.AccessToken);

        var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException($"fetch client fail response detail:{content}");
        }
        var results = JsonSerializer.Deserialize<List<KeycloakClientResponse>>(content, _jsonSerializerOptions);
        if (results is null || results.Count == 0)
        {
            throw new InvalidCastException("fetch keycloak client fail no content");
        }
        return results.First();
    }

    public async ValueTask<List<FetchRoleResponse>> FetchClientRolesAsync(FetchClientRolesRequest fetchClientRolesRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(fetchClientRolesRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/clients/{fetchClientRolesRequest.ClientUuid}/roles");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchClientRolesRequest.AccessToken);

        var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException($"fetch client fail response detail:{content}");
        }
        var results = JsonSerializer.Deserialize<List<FetchRoleResponse>>(content, _jsonSerializerOptions);
        if (results is null)
        {
            throw new InvalidCastException("fetch keycloak client fail no content");
        }
        return results;
    }

    public async ValueTask<List<FetchRoleResponse>> FetchUserClientRolesAsync(FetchUserClientRolesRequest fetchUserClientRolesRequest, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(fetchUserClientRolesRequest.AccessToken))
        {
            throw new UnauthorizedAccessException("AccessToken is required");
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{fetchUserClientRolesRequest.UserId}/role-mappings/clients/{fetchUserClientRolesRequest.ClientUuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchUserClientRolesRequest.AccessToken);

        var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException($"fetch client fail response detail:{content}");
        }
        var results = JsonSerializer.Deserialize<List<FetchRoleResponse>>(content, _jsonSerializerOptions);
        if (results is null)
        {
            throw new InvalidCastException("fetch keycloak client fail no content");
        }
        return results;
    }

    public async ValueTask AddUserClientRoleMappingAsync(string userId, string clientUuid, List<AddUserRoleMappingsRequest> addUserClientRoleMappingRequest, CancellationToken ct = default)
    {
        var tokenResponse = await AdminAccessToken(ct);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/role-mappings/clients/{clientUuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(addUserClientRoleMappingRequest, addUserClientRoleMappingRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidDataException($"add user role mapping fail response detail:{content}");
        }
    }

    public async ValueTask DeleteUserClientRoleMappingAsync(string userId, string clientUuid, List<DeleteUserRoleMappingsRequest> deleteUserClientRoleMappingRequest, CancellationToken ct = default)
    {
        var tokenResponse = await AdminAccessToken(ct);
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/role-mappings/clients/{clientUuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(deleteUserClientRoleMappingRequest, deleteUserClientRoleMappingRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidDataException($"add user role mapping fail response detail:{content}");
        }
    }
}
