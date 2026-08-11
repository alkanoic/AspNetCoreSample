namespace AspNetCoreSample.WebApi.Test;

public sealed class WeatherForecastTest
{
    [Test]
    [Category(nameof(WeatherForecastTest))]
    public async Task TemperatureF_CalculatesCorrectly()
    {
        var forecast = new WeatherForecast { TemperatureC = 0 };

        await Assert.That(forecast.TemperatureF).IsEqualTo(32);
    }
    [Test]
    [Category(nameof(WeatherForecastTest))]
    public async Task TemperatureF_WithPositiveCelsius()
    {
        var forecast = new WeatherForecast { TemperatureC = 100 };

        await Assert.That(forecast.TemperatureF).IsEqualTo(211);
    }
    [Test]
    [Category(nameof(WeatherForecastTest))]
    public async Task TemperatureF_WithNegativeCelsius()
    {
        var forecast = new WeatherForecast { TemperatureC = -40 };

        await Assert.That(forecast.TemperatureF).IsEqualTo(-39);
    }
}
