using DotNetAgentic.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAgentic.Controllers
{
    [ApiController]     // MARK this class as an API controller (enables auto features)
    [Route("[controller]")]     // Route: maps to /WeatherForecast (URL path)
    public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private readonly ILogger<WeatherForecastController> _logger = logger;

        // --------------------------------------------------------------------------------------
        // HTTP GET endpoint named "GetWeatherForecast", URL: GET http://localhost:5082/WeatherForecast
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
