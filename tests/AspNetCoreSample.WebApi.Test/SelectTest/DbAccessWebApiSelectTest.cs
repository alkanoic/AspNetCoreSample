using System.Net;
using System.Text.Json;

using AspNetCoreSample.DataModel.Models;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

[Collection(nameof(VerifySettingsFixtures))]
public sealed class DbAccessWebApiSelectTest : IClassFixture<DbFixture>, IDisposable
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly IServiceScope _serviceScope;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private readonly VerifySettings _verifySettings;

    public DbAccessWebApiSelectTest(DbFixture db, VerifySettingsFixture settingsFixture)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://+");
        // Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", "certificate.crt");
        // Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password", "password");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", db.DbConnectionString);
        _webApplicationFactory = new WebApplicationFactory<Program>();
        _serviceScope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _httpClient = _webApplicationFactory.CreateClient();
        _verifySettings = settingsFixture.VerifySettings;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _serviceScope.Dispose();
        _webApplicationFactory.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(DbAccessWebApiSelectTest))]
    public async Task GetDbAccessReturnsThreeNames()
    {
        // Given
        const string path = "api/dbaccess";

        // When
        var response = await _httpClient.GetAsync(path);
        var dbAccessStream = await response.Content.ReadAsStreamAsync();

        var names = await JsonSerializer.DeserializeAsync<IList<Name>>(dbAccessStream, JsonSerializerOptions);

        // Then
        await Verify(names, _verifySettings);
    }
}
