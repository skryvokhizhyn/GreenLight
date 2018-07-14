using System.Collections.Generic;
using System.Linq;

using Android.Locations;
using Android.OS;
using Android.Content;

using NLog;

namespace GreenLightTracker.Src
{
    class GpsLocationManager : Java.Lang.Object, ILocationListener
    {
        string m_locationProvider;
        LocationManager m_locationManager;

        private static Logger m_logger = NLog.LogManager.GetCurrentClassLogger();

        public delegate void NewGpsLocationHandler(GpsLocation l);
        public event NewGpsLocationHandler GpsLocationReceived;

        public GpsLocationManager(Context context)
        {
            InitializeLocationManager(context);
        }

        public void Start()
        {
            m_locationManager.RequestLocationUpdates(m_locationProvider, 0, 0, this);
        }

        public void Stop()
        {
            m_locationManager.RemoveUpdates(this);
        }

        // ILocationListener
        public void OnLocationChanged(Location location)
        {
            if (location == null)
            {
                m_logger.Error("Cannot identify location");
            }
            else
            {
                GpsLocationReceived?.Invoke(GpsLocationFactory.FromLocation(location));
            }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

        private void InitializeLocationManager(Context context)
        {
            m_locationManager = (LocationManager)context.GetSystemService(Context.LocationService);

            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = m_locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                m_locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                m_locationProvider = string.Empty;
            }

            m_logger.Trace("Location provider name is {0}", m_locationProvider);
        }
    }
}