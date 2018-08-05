
namespace GreenLightTracker.Src
{
    class EventsConnectionManager
    {
        private readonly GpsLocationManager m_locationManager;
        private readonly DBQueryManager m_queryManager;
        private readonly RoadTracker m_roadTracker;
        private readonly GreenLightTrackerActivity m_activity;

        public EventsConnectionManager(
            GpsLocationManager locationManager, 
            DBQueryManager queryManager, 
            RoadTracker roadTracker,
            GreenLightTrackerActivity activity)
        {
            m_locationManager = locationManager;
            m_queryManager = queryManager;
            m_roadTracker = roadTracker;
            m_activity = activity;
        }

        public void Disconnect()
        {
            m_locationManager.GpsLocationReceived -= m_activity.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived -= m_queryManager.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived -= m_roadTracker.OnGpsLocationReceived;

            m_roadTracker.NeighborsLost -= m_activity.OnNeighborsLost;
        }

        public void ConnectForCollection()
        {
            m_locationManager.GpsLocationReceived += m_queryManager.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived += m_activity.OnGpsLocationReceived;
        }

        public void ConnectForTracking()
        {
            m_locationManager.GpsLocationReceived += m_roadTracker.OnGpsLocationReceived;
            m_roadTracker.NeighborsLost += m_activity.OnNeighborsLost;
            m_roadTracker.PathPointFound += m_activity.OnRoadLightFound;
        }
    }
}