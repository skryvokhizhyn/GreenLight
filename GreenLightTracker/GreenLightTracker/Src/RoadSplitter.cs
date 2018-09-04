using System.Collections.Generic;
using System.Linq;

namespace GreenLightTracker.Src
{
    public class GpsCoordinateEqalityComparer : IEqualityComparer<GpsCoordinate>
    {
        private readonly float PRECISION = 1000.0f;

        public int GetHashCode(GpsCoordinate p)
        {
            int x = (int)(p.x * PRECISION);
            int y = (int)(p.y * PRECISION);
            int z = (int)(p.z * PRECISION);

            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            hash = hash * 23 + z.GetHashCode();
            return hash;
        }

        public bool Equals(GpsCoordinate lp, GpsCoordinate rp)
        {
            int lpx = (int)(lp.x * PRECISION);
            int lpy = (int)(lp.y * PRECISION);
            int lpz = (int)(lp.z * PRECISION);

            int rpx = (int)(rp.x * PRECISION);
            int rpy = (int)(rp.y * PRECISION);
            int rpz = (int)(rp.z * PRECISION);

            return lpx == rpx && lpy == rpy && lpz == rpz;
        }
    }

    public class RoadSplitter
    {
        private readonly float m_tolerance;

        public RoadSplitter(float tolerance)
        {
            m_tolerance = tolerance;
        }

        class PointAndDistance
        {
            public GpsCoordinate pointNeighbor;
            public GpsCoordinate pointPath;
            public float distance;
            public bool found;
        }

        public void Process(ICollection<PathData> paths)
        {
            if (paths == null)
                return;

            var mapper = new PathMapper(m_tolerance);

            var intersectionPoints = new HashSet<GpsCoordinate>(new GpsCoordinateEqalityComparer());

            foreach (var path in paths) 
            {
                var distancies = new Dictionary<int, PointAndDistance>();

                foreach (var point in path.Points)
                {
                    var neighbors = mapper.GetNearestPointsFiltered(point);

                    if (neighbors != null)
                    {
                        // update neighbors distances
                        foreach (var neighbor in neighbors)
                        {
                            var dist = PointUtils.GetDistance(point, neighbor.Point);

                            PointAndDistance distance;
                            if (distancies.TryGetValue(neighbor.PathId, out distance))
                            {
                                //distance.found = true;
                                if (distance.distance > dist)
                                {
                                    distance.distance = dist;
                                    distance.pointNeighbor = neighbor.Point;
                                    distance.pointPath = point;
                                }

                                distance.found = true;
                            }
                            else
                            {
                                distancies.Add(neighbor.PathId, new PointAndDistance
                                { pointNeighbor = neighbor.Point, pointPath = point, distance = dist, found = true });
                            }
                        }
                    }

                    var neighborsToRemove = new List<int>();
                    // cleanup removed neighbors
                    foreach (var item in distancies)
                    {
                        if (!item.Value.found)
                        {
                            intersectionPoints.Add(item.Value.pointNeighbor);
                            intersectionPoints.Add(item.Value.pointPath);
                            neighborsToRemove.Add(item.Key);
                        }

                        item.Value.found = false;
                    }

                    neighborsToRemove.ForEach(id => distancies.Remove(id));
                }

                // Path ended thus put all points to intersections
                foreach (var item in distancies)
                {
                    intersectionPoints.Add(item.Value.pointNeighbor);
                    intersectionPoints.Add(item.Value.pointPath);
                }

                mapper.PutPoints(PathPoint.CreateFromPathData(path));
            }

            var currentPathIndex = 0;
            var pathsList = (List<PathData>)paths;

            while (currentPathIndex < pathsList.Count)
            {
                // No sense to go from first point as we won't split it off in any case
                var currentPointIndex = 1;

                var pathData = pathsList[currentPathIndex];
                var points = pathData.Points;

                // We iterate to the point before last as we won't split it off
                while (currentPointIndex < points.Count - 1)
                {
                    var point = points[currentPointIndex];

                    if (intersectionPoints.Contains(point))
                    {
                        // Split from next point so we don't check the same point once again in tail
                        var tail = PointUtils.SplitPathDataAtIndex(pathData, currentPointIndex + 1);

                        if (tail != null)
                        {
                            pathsList.Add(tail);
                            break;
                        }
                    }

                    ++currentPointIndex;
                }

                ++currentPathIndex;
            }

            // TODO: ignore splitting neighbors if they are directed in differently (opposite road)
            // TODO: Don't split when it's first or last point on the path
        }
    }
}