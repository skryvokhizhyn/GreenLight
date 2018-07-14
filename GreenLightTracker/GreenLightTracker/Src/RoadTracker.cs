using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class RoadTracker
    {
        private readonly PathMapper m_mapper = null;
        private IList<PathPoint> m_neighbors = null;
        private GpsCoordinate m_previousPoint = null;

        public RoadTracker(PathMapper mapper)
        {
            m_mapper = mapper;
        }

        public void TrackPoint(GpsCoordinate point)
        {
            CleanupNeighbors(m_neighbors, m_previousPoint, point);

            if (m_neighbors == null || m_neighbors.Count == 0)
            {
                m_neighbors = m_mapper.GetNearestPointsFiltered(point);
            }

            m_previousPoint = point;
        }

        public static void CleanupNeighbors(IList<PathPoint> neighbors, GpsCoordinate currentPoint, GpsCoordinate nextPoint)
        {
            if (neighbors == null)
                return;

            var i = 0;

            while (i < neighbors.Count)
            {
                var p = neighbors[i];

                // Remove points w/o next
                if (p.Next == null)
                {
                    neighbors.RemoveAt(i);
                    continue;
                }

                // Remove too distant points
                if (PointUtils.GetDistance(p.Next.Point, nextPoint) > 10)
                {
                    neighbors.RemoveAt(i);
                    continue;
                }

                // Remove non-colinear directions
                var colinear = PointUtils.CheckColinear(
                    PointUtils.GetDirection(currentPoint, nextPoint),
                    PointUtils.GetDirection(p.Point, p.Next.Point),
                    10.0f);

                if (!colinear)
                {
                    neighbors.RemoveAt(i);
                    continue;
                }

                ++i;
            }
        }

        public static void PromoteNeighbors(IList<PathPoint> neighbors, GpsCoordinate point)
        {
            for (var i = 0; i < neighbors.Count; ++i)
            {
                neighbors[i] = neighbors[i].Next;
            }
        }

        public IList<PathPoint> GetNeighbors(bool sorted)
        {
            if (m_neighbors == null)
                return null;

            if (sorted)
            {
                ((List<PathPoint>)m_neighbors).Sort(
                (PathPoint l, PathPoint r) =>
                {
                    var distToL = PointUtils.GetDistance(l.Point, m_previousPoint);
                    var distToR = PointUtils.GetDistance(r.Point, m_previousPoint);

                    if (distToL < distToR)
                        return -1;
                    else if (distToL > distToR)
                        return 1;

                    return 0;
                });
            }

            return m_neighbors;
        }

        public IEnumerable<int> GetFollowedRoads()
        {

            return null;
        }
    }
}