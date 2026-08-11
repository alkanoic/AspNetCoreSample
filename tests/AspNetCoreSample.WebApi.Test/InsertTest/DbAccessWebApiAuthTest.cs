using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.WebApi.Services.Keycloak.Token;

using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreSample.WebApi.Test;

public sealed class DbAccessWebApiAuthTest : IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly IServiceScope _serviceScope;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public DbAccessWebApiAuthTest()
    {
        _webApplicationFactoryFixture = new WebApplicationFactoryFixture<Program>();
        _serviceScope = _webApplicationFactoryFixture.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _httpClient = _webApplicationFactoryFixture.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _serviceScope.Dispose();
        _webApplicationFactoryFixture.Dispose();
    }

    [Test]
    [Category(nameof(DbAccessWebApiAuthTest))]
    public async Task PostDbAccessAuth()
    {
        // Given
        const string path = "api/token/auth";

        // When
        var content = new StringContent(JsonSerializer.Serialize(new { userName = "admin", password = "admin" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        var dbAccessStream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(dbAccessStream, JsonSerializerOptions, CancellationToken.None);

        // Then
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(tokenResponse).IsNotNull();
        await Assert.That(tokenResponse.AccessToken).IsNotEmpty();
        await Assert.That(tokenResponse.RefreshToken).IsNotEmpty();

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        var test_response = await _httpClient.GetAsync("api/TokenTest/SampleAdmin?sample=asd", CancellationToken.None);

        var stream = await test_response.Content.ReadAsStringAsync(CancellationToken.None);
        await Assert.That(test_response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(stream).IsNotEmpty();
    }
}
