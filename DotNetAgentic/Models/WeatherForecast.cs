namespace DotNetAgentic.Models;

/// <summary>
/// Used to represent weather forecast data.
/// </summary>
public class WeatherForecast
{
    /// <summary>
    /// The date of the weather forecast.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// The temperature in Celsius.
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// The temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// A brief summary of the weather conditions.
    /// </summary>
    public string? Summary { get; set; }
}
