namespace WeatherAPPV4.Models
{
    public class WeatherViewModel
    {
        public DateTime Time { get; set; }
        public DateTime CurrentTime { get; set; }
        public float SwellHeight { get; set; }
        public float SwellPeriod { get; set; }
        public float SwellDirection{ get; set; }
        public float Airtemperature { get; set; }
        public float Watertemperature { get; set; }
        public float Winddirection { get; set; }
        public float Windspeed { get; set; }
    }
}
