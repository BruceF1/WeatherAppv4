namespace WeatherAPPV4.Models
{
    public class Meta
    {
        public int Cost { get; set; }
        public int DailyQuota { get; set; }
        public required string end { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string[] Params { get; set; }
        public int RequestCount { get; set; }
        public string Start { get; set; }
    }
}
