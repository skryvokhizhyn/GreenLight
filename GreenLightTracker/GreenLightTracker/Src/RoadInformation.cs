
namespace GreenLightTracker.Src
{
    class RoadInformation
    {
        private int m_currentRoadId = -1;

        public delegate void NewRoadFoundHandler(int id);
        public event NewRoadFoundHandler NewRoadFound;

        public delegate void CarMovedHandler(GpsCoordinate position);
        public event CarMovedHandler CarMoved;

        public const int InvalidRoadId = -1;

        public void OnPathPointFound(PathPoint p)
        {
            if (p == null)
                m_currentRoadId = InvalidRoadId;
            else if (p.PathId != m_currentRoadId)
                m_currentRoadId = p.PathId;
            else
                return;

            NewRoadFound(m_currentRoadId);
        }

        public void OnGpsLocationReceived(GpsLocation point)
        {
            CarMoved(GpsUtils.GetCoordinateFromLocation(point));
        }
    }
}