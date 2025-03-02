using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherAPPV4.Data;
using WeatherAPPV4.Models;

namespace WeatherAPPV4.Controllers
{
    public class HomeController : Controller
    {
        private readonly WeatherService _weatherService;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, WeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var latitude = 58.7984; 
                var longitude = 17.8081; 

                var weatherData = await _weatherService.GetWeatherAsync(latitude, longitude);
                var viewModel = weatherData.Hours.Select(hour => new WeatherViewModel
                {
                    Time = hour.time,
                    SwellHeight = hour.SwellHeight.noaa,
                    SwellPeriod = hour.SwellPeriod.noaa,
                    SwellDirection = hour.SwellDirection.noaa,
                    Airtemperature = hour.Airtemperature.noaa,
                    Watertemperature = hour.Watertemperature.noaa,
                    Winddirection = hour.Winddirection.noaa,
                    Windspeed = hour.Winddirection.noaa,
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return View("Error");
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