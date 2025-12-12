using System.Net;
using System.Text.Json;

using AspNetCoreSample.DataModel.Models;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class DbAccessWebApiSelectTest : IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly IServiceScope _serviceScope;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private readonly VerifySettingsFixture _verifySettingsFixture;

    public DbAccessWebApiSelectTest()
    {
        _webApplicationFactoryFixture = new WebApplicationFactoryFixture<Program>();
        _serviceScope = _webApplicationFactoryFixture.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _httpClient = _webApplicationFactoryFixture.CreateClient();
        _verifySettingsFixture = new VerifySettingsFixture();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _serviceScope.Dispose();
        _webApplicationFactoryFixture.Dispose();
    }

    [Test]
    [Category(nameof(DbAccessWebApiSelectTest))]
    public async Task GetDbAccessReturnsThreeNames()
    {
        // Given
        const string path = "api/dbaccess";

        // When
        var response = await _httpClient.GetAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), CancellationToken.None);
        var dbAccessStream = await response.Content.ReadAsStreamAsync(CancellationToken.None);

        var names = await JsonSerializer.DeserializeAsync<IList<Name>>(dbAccessStream, JsonSerializerOptions, CancellationToken.None);

        // Then
        await Verify(names, _verifySettingsFixture.VerifySettings);
    }
}
