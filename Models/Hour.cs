namespace WeatherAPPV4.Models
{
    public class Hour
    {
        public DateTime time { get; set; }
        public Swellheight SwellHeight { get; set; }
        public Swellperiod SwellPeriod { get; set; }
        public Swelldirection SwellDirection { get; set; }
        public Airtemperature Airtemperature { get; set; }
        public Watertemperature Watertemperature { get; set; }
        public Winddirection Winddirection { get; set; }
        public Windspeed Windspeed { get; set; }
    }
}
