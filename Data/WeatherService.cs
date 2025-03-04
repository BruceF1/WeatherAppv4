using Newtonsoft.Json;
using WeatherAPPV4.Models;

namespace WeatherAPPV4.Data
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;


        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiSettings:ApiKey"];
        }

        public async Task<RootObject> GetWeatherAsync(double latitude, double longitude)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.stormglass.io/v2/weather/point?lat={latitude}&lng={longitude}&params=swellPeriod,swellDirection,swellHeight,windSpeed,windDirection,airTemperature,waterTemperature");
                request.Headers.Add("Authorization", $"{_apiKey}");
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var weatherData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<RootObject>(weatherData);
                }
                else
                {
                    throw new Exception("Error fetching data");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching data", ex);
            }
        }

        public List<Hour> GetWeatherFromDB() 
        {
            try
            {
                List<RootObject> currentWeather = new List<RootObject>();
                using (var connection = new SqlConnection("AppDatabase"))
                {
                    connection.Open();
                    return connection.Query<Hour>("SELECT * FROM [WeatherAPP4].[dbo].[Weather]").ToList();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error fetching data from DB", ex);
            }
        }

    }
}
