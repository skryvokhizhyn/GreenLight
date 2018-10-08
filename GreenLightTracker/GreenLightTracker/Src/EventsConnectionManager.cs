using GreenLightTracker.Src.Activities;

namespace GreenLightTracker.Src
{
    class EventsConnectionManager
    {
        private readonly GpsLocationManager m_locationManager;
        private readonly DBQueryManager m_queryManager;
        private readonly RoadTracker m_roadTracker;
        private readonly RoadInformation m_roadInformation;
        private readonly GreenLightTrackerActivity m_activity;

        public EventsConnectionManager(
            GpsLocationManager locationManager, 
            DBQueryManager queryManager, 
            RoadTracker roadTracker,
            RoadInformation roadInformation,
            GreenLightTrackerActivity activity)
        {
            m_locationManager = locationManager;
            m_queryManager = queryManager;
            m_roadTracker = roadTracker;
            m_roadInformation = roadInformation;
            m_activity = activity;
        }

        public void Disconnect()
        {
            m_locationManager.GpsLocationReceived -= m_activity.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived -= m_queryManager.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived -= m_roadTracker.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived -= m_roadInformation.OnGpsLocationReceived;

            //m_roadTracker.PathPointFound -= m_activity.OnRoadLightFound;
            m_roadTracker.NeighborsUpdated -= m_activity.OnNeighborsUpdated;
            m_roadInformation.NewRoadFound -= m_activity.OnRoadFound;
            m_roadTracker.NewPathPointFound -= m_roadInformation.OnPathPointFound;
            m_roadInformation.CarMoved -= m_activity.OnCarMoved;
        }

        public void ConnectForCollection()
        {
            m_locationManager.GpsLocationReceived += m_queryManager.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived += m_activity.OnGpsLocationReceived;
        }

        public void ConnectForTracking()
        {
            m_locationManager.GpsLocationReceived += m_roadTracker.OnGpsLocationReceived;
            m_roadTracker.NeighborsUpdated += m_activity.OnNeighborsUpdated;
            //m_roadTracker.PathPointFound += m_activity.OnRoadLightFound;
            m_locationManager.GpsLocationReceived += m_roadInformation.OnGpsLocationReceived;
            m_roadTracker.NewPathPointFound += m_roadInformation.OnPathPointFound;
            m_roadInformation.NewRoadFound += m_activity.OnRoadFound;
            m_roadInformation.CarMoved += m_activity.OnCarMoved;
        }
    }
}