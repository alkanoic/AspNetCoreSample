using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;

using AspNetCoreSample.Mvc.Test;
using AspNetCoreSample.WebApi;
using AspNetCoreSample.WebApi.EfModels;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.MySql;

namespace AspNetCoreSample.Mvc.Test;

public sealed class DbAccessWebApiTest : IClassFixture<DbFixture>, IDisposable
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private readonly IServiceScope _serviceScope;

    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public DbAccessWebApiTest(DbFixture db)
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
    [Trait("Category", nameof(DbAccessWebApiTest))]
    public async Task Get_DbAccess_ReturnsThreeNames()
    {
        // Given
        const string path = "api/dbaccess";

        // When
        var response = await _httpClient.GetAsync(path);
        var dbAccessStream = await response.Content.ReadAsStreamAsync();

        var names = await JsonSerializer.DeserializeAsync<IList<Name>>(dbAccessStream, JsonSerializerOptions);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(names);
        Assert.Equal(3, names.Count);
        Assert.Equal("太郎", names[0].Name1);
        Assert.Equal("花子", names[1].Name1);
        Assert.Equal("令和", names[2].Name1);
    }

    [Fact]
    [Trait("Category", nameof(DbAccessWebApiTest))]
    public async Task Post_DbAccess_ReturnsThreeNames()
    {
        // Given
        const string path = "api/dbaccess";

        // When
        var content = new StringContent(JsonSerializer.Serialize(new Name() { Id = 0, Name1 = "string" }, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(path, content);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // [Fact]
    // [Trait("Category", nameof(WeatherForecastTest))]
    // public async Task Get_WeatherForecast_ReturnsThreeDays()
    // {
    //     // Given
    //     const int threeDays = 3;

    //     var weatherDataReadOnlyRepository = _serviceScope.ServiceProvider.GetRequiredService<IWeatherDataReadOnlyRepository>();

    //     // When
    //     var weatherForecast = await weatherDataReadOnlyRepository.GetAllAsync(string.Empty, string.Empty, DateTime.Today, DateTime.Today.AddDays(threeDays))
    //       .ConfigureAwait(true);

    //     // Then
    //     Assert.Equal(threeDays, weatherForecast.Count());
    // }
}
