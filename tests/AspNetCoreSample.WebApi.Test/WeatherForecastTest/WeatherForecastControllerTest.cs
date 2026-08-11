using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.WebApi.Test;

public sealed class WeatherForecastControllerTest : IDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _webApplicationFactoryFixture;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public WeatherForecastControllerTest()
    {
        _webApplicationFactoryFixture = new WebApplicationFactoryFixture<Program>();
        _httpClient = _webApplicationFactoryFixture.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _webApplicationFactoryFixture.Dispose();
    }

    [Test]
    [Category(nameof(WeatherForecastControllerTest))]
    public async Task GetWeatherForecastReturnsFiveItems()
    {
        const string path = "api/WeatherForecast";

        var response = await _httpClient.GetAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        var forecasts = await JsonSerializer.DeserializeAsync<List<WeatherForecast>>(stream, JsonSerializerOptions, CancellationToken.None);
        await Assert.That(forecasts).IsNotNull();
        await Assert.That(forecasts.Count).IsEqualTo(5);
        foreach (var forecast in forecasts)
        {
            await Assert.That(forecast.Summary).IsNotNull();
            await Assert.That(forecast.TemperatureC >= -20 && forecast.TemperatureC <= 55).IsTrue();
        }
    }

    [Test]
    [Category(nameof(WeatherForecastControllerTest))]
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
        var response = await _httpClient.PostAsync(new Uri(new Uri(_webApplicationFactoryFixture.HostUrl), path), content, CancellationToken.None);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        var result = await JsonSerializer.DeserializeAsync<WeatherForecast>(stream, JsonSerializerOptions, CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Date).IsEqualTo(input.Date);
        await Assert.That(result.TemperatureC).IsEqualTo(input.TemperatureC);
        await Assert.That(result.Summary).IsEqualTo(input.Summary);
    }
}
