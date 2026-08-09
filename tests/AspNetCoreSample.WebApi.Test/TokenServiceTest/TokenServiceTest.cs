using System.Net;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.WebApi.Options;
using AspNetCoreSample.WebApi.Services.Keycloak.Token;

using Microsoft.Extensions.Options;

namespace AspNetCoreSample.WebApi.Test;

public sealed class TokenServiceTest
{
    private static TokenService CreateService(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://keycloak") };
        var options = new FakeOptionsSnapshot<KeycloakOptions>(new KeycloakOptions
        {
            Authority = "http://keycloak",
            Audience = "test-audience",
            ClientId = "test-client",
            ClientSecret = "test-secret",
            TokenEndpoint = "/realms/Test/protocol/openid-connect/token",
            RevokeTokenEndpoint = "/realms/Test/protocol/openid-connect/logout",
            AdminUserName = "admin",
            AdminPassword = "admin",
            AdminTokenEndpoint = "/realms/master/protocol/openid-connect/token",
            AdminBaseAddress = "http://keycloak",
            TargetRealmName = "Test"
        });
        return new TokenService(httpClient, options);
    }

    [Fact]
    [Trait("Category", nameof(TokenServiceTest))]
    public async ValueTask AuthTokenAsync_ReturnsTokenResponse()
    {
        var expectedToken = new TokenResponse
        {
            AccessToken = "access-token-123",
            RefreshToken = "refresh-token-456",
            TokenType = "Bearer",
            ExpiresIn = 300,
            RefreshExpiresIn = 1800
        };
        var handler = new FakeHttpMessageHandler(req =>
        {
            var json = JsonSerializer.Serialize(expectedToken, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        });
        var service = CreateService(handler);

        var result = await service.AuthTokenAsync(new TokenRequest { UserName = "user", Password = "pass" }, TestContext.Current.CancellationToken);

        Assert.Equal("access-token-123", result.AccessToken);
        Assert.Equal("refresh-token-456", result.RefreshToken);
        Assert.Equal("Bearer", result.TokenType);
    }

    [Fact]
    [Trait("Category", nameof(TokenServiceTest))]
    public async ValueTask AuthTokenAsync_ThrowsOnNonSuccess()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            service.AuthTokenAsync(new TokenRequest { UserName = "user", Password = "wrong" }, TestContext.Current.CancellationToken).AsTask());
    }

    [Fact]
    [Trait("Category", nameof(TokenServiceTest))]
    public async ValueTask AuthTokenAsync_ThrowsOnNullResponse()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("null", Encoding.UTF8, "application/json") });
        var service = CreateService(handler);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            service.AuthTokenAsync(new TokenRequest { UserName = "user", Password = "pass" }, TestContext.Current.CancellationToken).AsTask());
    }

    [Fact]
    [Trait("Category", nameof(TokenServiceTest))]
    public async ValueTask RefreshTokenAsync_ReturnsTokenResponse()
    {
        var expectedToken = new TokenResponse
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            TokenType = "Bearer"
        };
        var handler = new FakeHttpMessageHandler(req =>
        {
            var json = JsonSerializer.Serialize(expectedToken, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        });
        var service = CreateService(handler);

        var result = await service.RefreshTokenAsync(new UpdateTokenRequest { RefreshToken = "old-refresh" }, TestContext.Current.CancellationToken);

        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);
    }

    [Fact]
    [Trait("Category", nameof(TokenServiceTest))]
    public async ValueTask RefreshTokenAsync_ThrowsOnNonSuccess()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            service.RefreshTokenAsync(new UpdateTokenRequest { RefreshToken = "invalid" }, TestContext.Current.CancellationToken).AsTask());
    }

    [Fact]
    [Trait("Category", nameof(TokenServiceTest))]
    public async ValueTask RevokeTokenAsync_CompletesOnSuccess()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.NoContent));
        var service = CreateService(handler);

        await service.RevokeTokenAsync(new RevokeTokenRequest { RefreshToken = "token-to-revoke" }, TestContext.Current.CancellationToken);
    }

    [Fact]
    [Trait("Category", nameof(TokenServiceTest))]
    public async ValueTask RevokeTokenAsync_ThrowsOnNonSuccess()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.BadRequest));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            service.RevokeTokenAsync(new RevokeTokenRequest { RefreshToken = "invalid" }, TestContext.Current.CancellationToken).AsTask());
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }

    private sealed class FakeOptionsSnapshot<T> : IOptionsSnapshot<T> where T : class
    {
        public T Value { get; }

        public FakeOptionsSnapshot(T value)
        {
            Value = value;
        }

        public T Get(string? name) => Value;
    }
}
