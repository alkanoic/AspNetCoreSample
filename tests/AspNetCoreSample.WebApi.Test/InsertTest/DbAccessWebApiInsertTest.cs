using System.Net;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.DataModel.Models;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class DbAccessWebApiInsertTest : IClassFixture<WebApplicationFactoryFixture<Program>>, IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;

    private readonly IServiceScope _serviceScope;

    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public DbAccessWebApiInsertTest(WebApplicationFactoryFixture<Program> webApplicationFactoryFixture)
    {
        _webApplicationFactoryFixture = webApplicationFactoryFixture;
        _serviceScope = _webApplicationFactoryFixture.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _httpClient = _webApplicationFactoryFixture.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _serviceScope.Dispose();
        _webApplicationFactoryFixture.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(DbAccessWebApiInsertTest))]
    public async Task PostDbAccessRegisterName()
    {
        // Given
        const string path = "api/dbaccess";

        // When
        var content = new StringContent(JsonSerializer.Serialize(new Name() { Id = 0, Name1 = "string" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
