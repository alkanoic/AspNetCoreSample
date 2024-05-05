using System.Net;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.WebApi.EfModels;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class DbAccessWebApiInsertTest : IClassFixture<DbFixture>, IDisposable
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private readonly IServiceScope _serviceScope;

    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public DbAccessWebApiInsertTest(DbFixture db)
    {
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
    [Trait("Category", nameof(DbAccessWebApiInsertTest))]
    public async Task Post_DbAccess_RegisterName()
    {
        // Given
        const string path = "api/dbaccess";

        // When
        var content = new StringContent(JsonSerializer.Serialize(new Name() { Id = 0, Name1 = "string" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(path, content);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
