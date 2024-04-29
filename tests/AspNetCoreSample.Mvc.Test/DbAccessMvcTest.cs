using System.Net;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.Mvc;
using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.Mvc.Test;

public sealed class DbAccessMvcTest : IClassFixture<DbFixture>, IDisposable
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private readonly IServiceScope _serviceScope;

    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public DbAccessMvcTest(DbFixture db)
    {
        // Instead of using environment variables to bootstrap our application configuration, we can implement a custom WebApplicationFactory<TEntryPoint>
        // that overrides the ConfigureWebHost(IWebHostBuilder) method to add a WeatherDataContext to the service collection.
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://+");
        // Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", "certificate.crt");
        // Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password", "password");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", db.DbConnectionString);
        _webApplicationFactory = new WebApplicationFactory<Program>();
        _serviceScope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _httpClient = _webApplicationFactory.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _serviceScope.Dispose();
        _webApplicationFactory.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(DbAccessMvcTest))]
    public async Task Get_DbAccess_ReturnsThreeNames()
    {
        // Given
        const string path = "/";

        // When
        var response = await _httpClient.GetAsync(path);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
