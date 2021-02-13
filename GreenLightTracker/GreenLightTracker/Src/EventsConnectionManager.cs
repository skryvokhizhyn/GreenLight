using GreenLightTracker.Src.Activities;

namespace GreenLightTracker.Src
{
    class EventsConnectionManager
    {
        private readonly GpsLocationManager m_locationManager;
        private readonly GpsLocationBuffer m_locationBuffer;
        private readonly GreenLightTrackerActivity m_activity;

        public EventsConnectionManager(
            GpsLocationManager locationManager,
            GpsLocationBuffer queryManager, 
            GreenLightTrackerActivity activity)
        {
            m_locationManager = locationManager;
            m_locationBuffer = queryManager;
            m_activity = activity;
        }

        public void Disconnect()
        {
            m_locationManager.GpsLocationReceived -= m_activity.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived -= m_locationBuffer.OnGpsLocationReceived;
        }

        public void ConnectForCollection()
        {
            m_locationManager.GpsLocationReceived += m_locationBuffer.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived += m_activity.OnGpsLocationReceived;
        }

        public void ConnectForTracking()
        {
        }
    }
}