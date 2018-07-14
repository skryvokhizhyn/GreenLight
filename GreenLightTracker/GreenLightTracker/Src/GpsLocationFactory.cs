using Android.Locations;

namespace GreenLightTracker.Src
{
    class GpsLocationFactory
    {
        public static GpsLocation FromLocation(Location location) => new GpsLocation()
        {
            Longitude = location.Longitude,
            Latitude = location.Latitude,
            Altitude = location.Altitude,
            Speed = location.Speed
        };
    }
}