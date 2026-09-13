using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using AspNetCoreSample.Mvc.Logging;
using AspNetCoreSample.Mvc.Models;
using AspNetCoreSample.Mvc.Options;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCoreSample.Mvc.Controllers;

public class KeycloakUserController : Controller
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    private readonly ILogger<KeycloakUserController> _logger;
    private readonly KeycloakOptions _keycloakOptions;
    private readonly WebApiOption _webApiOption;

    public KeycloakUserController(
        ILogger<KeycloakUserController> logger,
        IOptions<KeycloakOptions> keycloakOptions,
        IOptions<WebApiOption> webApiOption)
    {
        _logger = logger;
        _keycloakOptions = keycloakOptions.Value;
        _webApiOption = webApiOption.Value;
    }

    [Logging]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Logging]
    public async Task<IActionResult> CreateAndLogin(
        string username,
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken ct)
    {
        try
        {
            // 1. Create user via WebAPI
            var created = await CreateUserViaWebApi(username, firstName, lastName, email, password, ct);
            if (!created)
            {
                TempData["Error"] = "ユーザーの作成に失敗しました";
                return RedirectToAction(nameof(Index));
            }

            // 2. Get tokens from Keycloak using ROPC (Resource Owner Password Credentials)
            var tokens = await GetTokensFromKeycloak(username, password, ct);
            if (tokens == null)
            {
                TempData["Error"] = "Keycloakからのトークン取得に失敗しました。Direct Access Grantsが有効か確認してください。";
                return RedirectToAction(nameof(Index));
            }

            // 3. Create ClaimsPrincipal from ID token
            var principal = CreatePrincipalFromIdToken(tokens.IdToken);
            if (principal == null)
            {
                TempData["Error"] = "IDトークンの解析に失敗しました";
                return RedirectToAction(nameof(Index));
            }

            // 4. Sign in the user
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                });

            if (_logger.IsEnabled(LogLevel.Information))
            {
                // _logger.LogInformation("User {Username} created and logged in via ROPC", username);
            }

            return RedirectToAction("Index", "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating and logging in user {Username}", username);
            TempData["Error"] = $"エラーが発生しました: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task<bool> CreateUserViaWebApi(
        string username,
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken ct)
    {
        var handler = new HttpClientHandler();
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        using var client = new HttpClient(handler);
        client.BaseAddress = new Uri(_webApiOption.WebApiBaseUrl);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var userData = new
        {
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };

        var json = JsonSerializer.Serialize(userData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/Keycloak/CreateUserAdmin", content, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("WebAPI CreateUser failed: {StatusCode} - {Content}", response.StatusCode, errorContent);
        }
        return response.IsSuccessStatusCode;
    }

    private async Task<TokenResponse?> GetTokensFromKeycloak(string username, string password, CancellationToken ct)
    {
        var handler = new HttpClientHandler();
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        using var client = new HttpClient(handler);
        var tokenEndpoint = _keycloakOptions.TokenEndpoint;

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _keycloakOptions.ClientId,
            ["client_secret"] = _keycloakOptions.ClientSecret,
            ["username"] = username,
            ["password"] = password,
            ["scope"] = "openid profile"
        };

        var content = new FormUrlEncodedContent(formData);
        var response = await client.PostAsync(tokenEndpoint, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Keycloak token request failed: {StatusCode} - {Content}", response.StatusCode, errorContent);
            return null;
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TokenResponse>(json, JsonOptions);
    }

    private ClaimsPrincipal? CreatePrincipalFromIdToken(string idToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(idToken);

            var claims = jwtToken.Claims.ToList();

            // Ensure we have a name identifier
            if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            {
                var sub = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                if (!string.IsNullOrEmpty(sub))
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));
                }
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create principal from ID token");
            return null;
        }
    }

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;
    }
}
