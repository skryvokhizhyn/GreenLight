namespace GreenLightTracker.Src
{
    public struct GpsLocation
    {
        public int Timestamp { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }
        public float Speed { get; set; }
    }
}