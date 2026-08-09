using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class WeatherForecastControllerTest : IClassFixture<WebApplicationFactoryFixture<Program>>, IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public WeatherForecastControllerTest(WebApplicationFactoryFixture<Program> webApplicationFactoryFixture)
    {
        _webApplicationFactoryFixture = webApplicationFactoryFixture;
        _httpClient = _webApplicationFactoryFixture.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(WeatherForecastControllerTest))]
    public async Task GetWeatherForecastReturnsFiveItems()
    {
        const string path = "api/WeatherForecast";

        var response = await _httpClient.GetAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        var forecasts = await JsonSerializer.DeserializeAsync<List<WeatherForecast>>(stream, JsonSerializerOptions, TestContext.Current.CancellationToken);
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Count);
        foreach (var forecast in forecasts)
        {
            Assert.NotNull(forecast.Summary);
            Assert.True(forecast.TemperatureC >= -20 && forecast.TemperatureC <= 55);
        }
    }

    [Fact]
    [Trait("Category", nameof(WeatherForecastControllerTest))]
    public async Task PostWeatherForecastReturnsSameData()
    {
        const string path = "api/WeatherForecast";

        var input = new WeatherForecast
        {
            Date = new DateTime(2024, 1, 1),
            TemperatureC = 25,
            Summary = "Warm"
        };
        var content = new StringContent(JsonSerializer.Serialize(input, JsonSerializerOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        var result = await JsonSerializer.DeserializeAsync<WeatherForecast>(stream, JsonSerializerOptions, TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(input.Date, result.Date);
        Assert.Equal(input.TemperatureC, result.TemperatureC);
        Assert.Equal(input.Summary, result.Summary);
    }
}
