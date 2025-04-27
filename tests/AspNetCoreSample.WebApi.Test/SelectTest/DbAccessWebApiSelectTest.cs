using System.Net;
using System.Text.Json;

using AspNetCoreSample.DataModel.Models;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

[Collection(nameof(VerifySettingsFixtures))]
public sealed class DbAccessWebApiSelectTest : IClassFixture<WebApplicationFactoryFixture<Program>>, IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly IServiceScope _serviceScope;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private readonly VerifySettings _verifySettings;

    public DbAccessWebApiSelectTest(WebApplicationFactoryFixture<Program> webApplicationFactoryFixture, VerifySettingsFixture settingsFixture)
    {
        _webApplicationFactoryFixture = webApplicationFactoryFixture;
        _serviceScope = _webApplicationFactoryFixture.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _httpClient = _webApplicationFactoryFixture.CreateClient();
        _verifySettings = settingsFixture.VerifySettings;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _serviceScope.Dispose();
        _webApplicationFactoryFixture.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(DbAccessWebApiSelectTest))]
    public async Task GetDbAccessReturnsThreeNames()
    {
        // Given
        const string path = "api/dbaccess";

        // When
        var response = await _httpClient.GetAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), TestContext.Current.CancellationToken);
        var dbAccessStream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);

        var names = await JsonSerializer.DeserializeAsync<IList<Name>>(dbAccessStream, JsonSerializerOptions, TestContext.Current.CancellationToken);

        // Then
        await Verify(names, _verifySettings);
    }
}
