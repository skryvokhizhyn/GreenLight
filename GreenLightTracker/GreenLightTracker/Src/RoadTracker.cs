using System;
using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class RoadTracker
    {
        private PathMapper m_mapper = null;
        private List<PathPoint> m_neighbors = null;
        private GpsCoordinate m_previousPoint = null;

        public delegate void NewPathPointHandler(PathPoint p, int count);
        public event NewPathPointHandler PathPointFound;

        public RoadTracker(PathMapper mapper = null)
        {
            m_mapper = mapper;
        }

        public void SetMapper(PathMapper mapper)
        {
            m_mapper = mapper;
        }

        public bool IsInitialized()
        {
            return m_mapper != null;
        }

        public void OnGpsLocationReceived(GpsLocation point)
        {
            TrackPointAndNotify(GpsUtils.GetCoordinateFromLocation(point));
        }

        public PathPoint TrackPoint(GpsCoordinate point)
        {
            if (point == null)
                return null;

            if (m_previousPoint != null)
            {
                CleanupNeighbors(m_neighbors, m_previousPoint, point, m_mapper.GetTolerance());

                if (m_neighbors == null || m_neighbors.Count == 0)
                {
                    m_neighbors = m_mapper.GetNearestPointsFiltered(point);

                    if (m_neighbors != null)
                    {
                        int a = 0;
                        a++;
                    }
                }
                else
                {
                    PromoteNeighbors(m_neighbors, point);
                }
            }
            else
            {
                m_neighbors = m_mapper.GetNearestPointsFiltered(point);
            }

            m_previousPoint = point;

            if (m_neighbors == null || m_neighbors.Count == 0)
            {
                return null;
            }
            else
            {
                return GetClosestNeighbor();
            }
        }

        public void TrackPointAndNotify(GpsCoordinate point)
        {
            var pathPoint = TrackPoint(point);

            PathPointFound(pathPoint, m_neighbors != null ? m_neighbors.Count : 0);
        }

        public static void CleanupNeighbors(IList<PathPoint> neighbors, GpsCoordinate currentPoint, GpsCoordinate nextPoint, float tolerance)
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
                if (PointUtils.GetDistance(p.Point, nextPoint) > tolerance)
                {
                    neighbors.RemoveAt(i);
                    continue;
                }

                // Remove non-colinear directions
                var colinear = PointUtils.CheckColinear(
                    PointUtils.GetDirection(currentPoint, nextPoint),
                    PointUtils.GetDirection(p.Point, p.Next.Point),
                    30.0f);

                if (!colinear)
                {
                    neighbors.RemoveAt(i);
                    continue;
                }

                ++i;
            }
        }

        public static void PromoteNeighbors(List<PathPoint> neighbors, GpsCoordinate point)
        {
            if (neighbors == null || point == null)
                return;

            for (var i = 0; i < neighbors.Count; ++i)
            {
                var neighbor = neighbors[i];
                while (neighbor != null)
                {
                    if (neighbor.Next == null)
                    {
                        neighbor = null;
                        break;
                    }

                    var v1 = new GpsCoordinate
                    {
                        x = neighbor.Next.Point.x - point.x,
                        y = neighbor.Next.Point.y - point.y,
                        z = neighbor.Next.Point.z - point.z
                    };

                    var v2 = new GpsCoordinate
                    {
                        x = neighbor.Next.Point.x - neighbor.Point.x,
                        y = neighbor.Next.Point.y - neighbor.Point.y,
                        z = neighbor.Next.Point.z - neighbor.Point.z
                    };

                    var angle = PointUtils.GetAngleBetween(v1, v2);

                    if (Math.Abs(angle) < 90)
                        break;

                    neighbor = neighbor.Next;
                }

                neighbors[i] = neighbor;
            }

            neighbors.RemoveAll(n => n == null);
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

        public PathPoint GetClosestNeighbor()
        {
            var neighbors = GetNeighbors(true);

            if (neighbors == null || neighbors.Count == 0)
                return null;

            return neighbors[0];
        }
    }
}