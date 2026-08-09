namespace AspNetCoreSample.WebApi.Test;

public sealed class WeatherForecastTest
{
    [Fact]
    [Trait("Category", nameof(WeatherForecastTest))]
    public void TemperatureF_CalculatesCorrectly()
    {
        var forecast = new WeatherForecast { TemperatureC = 0 };

        Assert.Equal(32, forecast.TemperatureF);
    }

    [Fact]
    [Trait("Category", nameof(WeatherForecastTest))]
    public void TemperatureF_WithPositiveCelsius()
    {
        var forecast = new WeatherForecast { TemperatureC = 100 };

        Assert.Equal(211, forecast.TemperatureF);
    }

    [Fact]
    [Trait("Category", nameof(WeatherForecastTest))]
    public void TemperatureF_WithNegativeCelsius()
    {
        var forecast = new WeatherForecast { TemperatureC = -40 };

        Assert.Equal(-39, forecast.TemperatureF);
    }
}
