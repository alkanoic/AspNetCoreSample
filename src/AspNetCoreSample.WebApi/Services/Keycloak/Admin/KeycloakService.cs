
using AspNetCoreSample.WebApi.Models.Keycloak;
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
    ValueTask UpdateUserByUsernameAsync(string username, UpdateUserRequest updateUserRequest);
    ValueTask ChangePasswordAsync(string userId, ChangePasswordRequest changePasswordRequest);
    ValueTask ChangePasswordByUsernameAsync(string username, ChangePasswordRequest changePasswordRequest);
    ValueTask ResetPasswordByEmailAsync(ResetPasswordByEmailRequest resetPasswordByEmailRequest);
    ValueTask DeleteUserAsync(DeleteUserRequest deleteUserRequest);
    ValueTask<List<FetchRoleResponse>> FetchRolesAsync();
    ValueTask<List<FetchRoleResponse>> FetchUserRoleMappingsAsync(FetchUserRoleMappingsRequest fetchUserRoleMappingsRequest);
    ValueTask AddUserRoleMappingAsync(string userId, List<AddUserRoleMappingsRequest> addUserRoleMappingsRequest);
    ValueTask DeleteUserRoleMappingAsync(string userId, List<DeleteUserRoleMappingsRequest> deleteUserRoleMappingsRequest);
    ValueTask<List<KeycloakClientResponse>> FetchClientsAsync(FetchClientsRequest fetchClientsRequest);
    ValueTask<KeycloakClientResponse> FetchClientAsync(FetchClientRequest fetchClientRequest);
    ValueTask<List<FetchRoleResponse>> FetchClientRolesAsync(FetchClientRolesRequest fetchClientRolesRequest);
    ValueTask<List<FetchRoleResponse>> FetchUserClientRolesAsync(FetchUserClientRolesRequest fetchUserClientRolesRequest);
    ValueTask AddUserClientRoleMappingAsync(string userId, string clientUuid, List<AddUserRoleMappingsRequest> addUserClientRoleMappingRequest);
    ValueTask DeleteUserClientRoleMappingAsync(string userId, string clientUuid, List<DeleteUserRoleMappingsRequest> deleteUserClientRoleMappingRequest);
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

    /// <summary>
    /// ユーザー名からユーザー情報を取得する
    /// ユーザー名は完全一致で取得
    /// </summary>
    public async ValueTask<FetchUserResponse> FetchUserAsync(FetchUserRequest fetchUserRequest)
    {
        if (string.IsNullOrEmpty(fetchUserRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            fetchUserRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users?exact=true&username={fetchUserRequest.Username}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchUserRequest.AccessToken);

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

    /// <summary>
    /// ユーザーを作成する
    /// </summary>
    public async ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest)
    {
        if (string.IsNullOrEmpty(createUserRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            createUserRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", createUserRequest.AccessToken);
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

    /// <summary>
    /// ユーザー情報を更新する
    /// </summary>
    public async ValueTask UpdateUserAsync(string userId, UpdateUserRequest updateUserRequest)
    {
        if (string.IsNullOrEmpty(updateUserRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            updateUserRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", updateUserRequest.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(updateUserRequest, updateUserRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"update user fail response detail:{content}");
        }
    }

    /// <summary>
    /// ユーザー名でユーザー情報を更新
    /// </summary>
    public async ValueTask UpdateUserByUsernameAsync(string username, UpdateUserRequest updateUserRequest)
    {
        if (string.IsNullOrEmpty(updateUserRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            updateUserRequest.AccessToken = tokenResponse.AccessToken;
        }
        var fetchUserResponse = await FetchUserAsync(new FetchUserRequest() { Username = username, AccessToken = updateUserRequest.AccessToken });
        await UpdateUserAsync(fetchUserResponse.Id, updateUserRequest);
    }

    /// <summary>
    /// ユーザーのパスワード変更
    /// </summary>
    public async ValueTask ChangePasswordAsync(string userId, ChangePasswordRequest changePasswordRequest)
    {
        if (string.IsNullOrEmpty(changePasswordRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            changePasswordRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/reset-password");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", changePasswordRequest.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(changePasswordRequest.Credential, changePasswordRequest.Credential.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"change password fail response detail:{content}");
        }
    }

    /// <summary>
    /// ユーザー名でユーザーのパスワード変更
    /// </summary>
    public async ValueTask ChangePasswordByUsernameAsync(string username, ChangePasswordRequest changePasswordRequest)
    {
        if (string.IsNullOrEmpty(changePasswordRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            changePasswordRequest.AccessToken = tokenResponse.AccessToken;
        }
        var fetchUserResponse = await FetchUserAsync(new FetchUserRequest() { AccessToken = changePasswordRequest.AccessToken, Username = username });
        await ChangePasswordAsync(fetchUserResponse.Id, changePasswordRequest);
    }

    public async ValueTask ResetPasswordByEmailAsync(ResetPasswordByEmailRequest resetPasswordByEmailRequest)
    {
        if (string.IsNullOrEmpty(resetPasswordByEmailRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            resetPasswordByEmailRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{resetPasswordByEmailRequest.UserId}/reset-password-email");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", resetPasswordByEmailRequest.AccessToken);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"reset password email fail response detail:{content}");
        }
    }

    public async ValueTask DeleteUserAsync(DeleteUserRequest deleteUserRequest)
    {
        if (string.IsNullOrEmpty(deleteUserRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            deleteUserRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{deleteUserRequest.UserId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", deleteUserRequest.AccessToken);

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
        if (string.IsNullOrEmpty(fetchUserRoleMappingsRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            fetchUserRoleMappingsRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{fetchUserRoleMappingsRequest.UserId}/role-mappings/realm");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchUserRoleMappingsRequest.AccessToken);

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

    public async ValueTask<List<KeycloakClientResponse>> FetchClientsAsync(FetchClientsRequest fetchClientsRequest)
    {
        if (string.IsNullOrEmpty(fetchClientsRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            fetchClientsRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/clients");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchClientsRequest.AccessToken);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
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

    public async ValueTask<KeycloakClientResponse> FetchClientAsync(FetchClientRequest fetchClientRequest)
    {
        if (string.IsNullOrEmpty(fetchClientRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            fetchClientRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/clients?clientId={fetchClientRequest.ClientId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchClientRequest.AccessToken);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
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

    public async ValueTask<List<FetchRoleResponse>> FetchClientRolesAsync(FetchClientRolesRequest fetchClientRolesRequest)
    {
        if (string.IsNullOrEmpty(fetchClientRolesRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            fetchClientRolesRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/clients/{fetchClientRolesRequest.ClientUuid}/roles");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchClientRolesRequest.AccessToken);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
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

    public async ValueTask<List<FetchRoleResponse>> FetchUserClientRolesAsync(FetchUserClientRolesRequest fetchUserClientRolesRequest)
    {
        if (string.IsNullOrEmpty(fetchUserClientRolesRequest.AccessToken))
        {
            var tokenResponse = await AdminAccessToken();
            fetchUserClientRolesRequest.AccessToken = tokenResponse.AccessToken;
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{fetchUserClientRolesRequest.UserId}/role-mappings/clients/{fetchUserClientRolesRequest.ClientUuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fetchUserClientRolesRequest.AccessToken);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
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
        // GET /admin/realms/{realm}/users/{user-id}/role-mappings/clients/{client}
    }

    public async ValueTask AddUserClientRoleMappingAsync(string userId, string clientUuid, List<AddUserRoleMappingsRequest> addUserClientRoleMappingRequest)
    {
        // POST /admin/realms/{realm}/users/{user-id}/role-mappings/clients/{client}
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/role-mappings/clients/{clientUuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(addUserClientRoleMappingRequest, addUserClientRoleMappingRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"add user role mapping fail response detail:{content}");
        }
    }

    public async ValueTask DeleteUserClientRoleMappingAsync(string userId, string clientUuid, List<DeleteUserRoleMappingsRequest> deleteUserClientRoleMappingRequest)
    {
        // DELETE /admin/realms/{realm}/users/{user-id}/role-mappings/clients/{client}
        var tokenResponse = await AdminAccessToken();
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}admin/realms/{_keycloakOptions.TargetRealmName}/users/{userId}/role-mappings/clients/{clientUuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(deleteUserClientRoleMappingRequest, deleteUserClientRoleMappingRequest.GetType(), _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidDataException($"add user role mapping fail response detail:{content}");
        }
    }
}
