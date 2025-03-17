using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data.SqlClient;
using WeatherAPPV4.Data;
using WeatherAPPV4.Models;
using Newtonsoft.Json;

namespace WeatherAPPV4.Controllers
{
    public class HomeController : Controller
    {
        private readonly WeatherService _weatherService;
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;

        public HomeController(ILogger<HomeController> logger, WeatherService weatherService, IConfiguration configuration)
        {
            _logger = logger;
            _weatherService = weatherService;
            _connectionString = configuration["ConnectionStrings:AppDatabase"];
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentTime = DateTime.Now;
                var dbWeatherData = GetWeatherFromDB();

                if (dbWeatherData != null && dbWeatherData.Any())
                {
                    var viewModel = dbWeatherData.Select(hour => new WeatherViewModel
                    {
                        CurrentTime = currentTime,
                        Time = hour.Time != null ? hour.Time : DateTime.MinValue,
                        SwellHeight = hour.SwellHeight != null ? hour.SwellHeight : 0.0f,
                        SwellPeriod = hour.SwellPeriod != null ? hour.SwellPeriod : 0.0f,
                        SwellDirection = hour.SwellDirection != null ? hour.SwellDirection : 0.0f,
                        Airtemperature = hour.Airtemperature != null ? hour.Airtemperature : 0.0f,
                        Watertemperature = hour.Watertemperature != null ? hour.Watertemperature : 0.0f,
                        Winddirection = hour.Winddirection != null ? hour.Winddirection : 0.0f,
                        Windspeed = hour.Windspeed != null ? hour.Windspeed : 0.0f,
                    }).ToList();

                    return View(viewModel);
                }

                // If no data in DB, fetch from API and update DB
                var latitude = -33.55;
                var longitude = 18.25;
                var weatherData = await _weatherService.GetWeatherAsync(latitude, longitude);

                if (weatherData != null && weatherData.Hours.Any())
                {
                    UpdateWeatherDB(weatherData.Hours); // Save API data to DB

                    var viewModel = weatherData.Hours.Select(hour => new WeatherViewModel
                    {
                        CurrentTime = currentTime,
                        Time = hour.time,
                        SwellHeight = hour.SwellHeight?.noaa ?? 0,
                        SwellPeriod = hour.SwellPeriod?.noaa ?? 0,
                        SwellDirection = hour.SwellDirection?.noaa ?? 0,
                        Airtemperature = hour.Airtemperature?.noaa ?? 0,
                        Watertemperature = hour.Watertemperature?.noaa ?? 0,
                        Winddirection = hour.Winddirection?.noaa ?? 0,
                        Windspeed = hour.Windspeed?.noaa ?? 0
                    }).ToList();

                    return View(viewModel);
                }

                return View("Error"); // If API also fails, show error page
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return View("Error");
            }
        }

        private List<WeatherViewModel> GetWeatherFromDB()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var result = connection.Query<WeatherViewModel>("SELECT TOP 1 Entrydate AS [TIME],ROUND([SwellHeight], 2) AS SwellHeight,ROUND([SwellPeriod], 2) AS SwellPeriod,ROUND([SwellDirection], 2) AS SwellDirection,ROUND(AirTemperature, 2) AS AirTemperature,ROUND([WaterTemperature], 2) AS WaterTemperature,ROUND([WindDirection], 2) AS WindDirection, ROUND([WindSpeed], 2) AS WindSpeed FROM [WeatherAPP4].[dbo].[Weather] ORDER BY Entrydate asc;").ToList();
                    _logger.LogInformation($"DB Result: {JsonConvert.SerializeObject(result)}");
                    return result;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching data from DB: {ex.Message}");
                return new List<WeatherViewModel>();
            }
        }

        private void UpdateWeatherDB(List<Hour> weatherData)
        {
            try
            {
                var currentTime = DateTime.Now;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    foreach (var hour in weatherData)
                    {
                        var query = @"
INSERT INTO [WeatherAPP4].[dbo].[Weather] (
	[EntryDate]
	,SwellHeight
	,SwellPeriod
	,SwellDirection
	,AirTemperature
	,[WaterTemperature]
	,[WindDirection]
	,[WindSpeed]
	)
VALUES (
	@time
	,@SwellHeight
	,@SwellPeriod
	,@SwellDirection
	,@Airtemperature
	,@Watertemperature
	,@Winddirection
	,@Windspeed
	)";

                        connection.Execute(query, new
                        {
                            CurrentTime = currentTime,
                            time = hour.time,
                            Airtemperature = hour.Airtemperature?.noaa ?? 0,
                            SwellDirection = hour.SwellDirection?.noaa ?? 0,
                            SwellHeight = hour.SwellHeight?.noaa ?? 0,
                            SwellPeriod = hour.SwellPeriod?.noaa ?? 0,
                            Watertemperature = hour.Watertemperature?.noaa ?? 0,
                            Winddirection = hour.Winddirection?.noaa ?? 0,
                            Windspeed = hour.Windspeed?.noaa ?? 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating DB: {ex.Message}");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("forecast")]
        public async Task<IActionResult> GetWeather(double latitude, double longitude)
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherAsync(latitude, longitude);
                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching forecast: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
