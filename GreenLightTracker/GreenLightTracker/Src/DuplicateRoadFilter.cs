using System.Linq;
using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class DuplicateRoadFilter
    {
        private readonly float m_pointsTolerance;

        public DuplicateRoadFilter(float pointsTolerance)
        {
            m_pointsTolerance = pointsTolerance;
        }

        public void Process(ICollection<PathData> paths, PathConnections pathConnections = null)
        {
            if (paths == null)
                return;

            var mapper = new PathMapper(m_pointsTolerance);

            var pathIndex = 0;

            var pathsList = (List<PathData>)paths;
            var newPathsListTotal = new List<PathData>();

            while (pathIndex < paths.Count)
            {
                var pathData = pathsList[pathIndex];

                var pointIndex = 0;

                var pointToNeighborPathId = new MultiValueDictionary<GpsCoordinate, int>();
                var allColinearPathIds = new HashSet<int>();

                List<int> prevColinearPathIds = null;

                while (pointIndex < pathData.Points.Count)
                {
                    var point = pathData.Points[pointIndex];

                    var nearestPoints = mapper.GetNearestPointsFiltered(point);

                    var colinearPathIds = GetColinearPathIds(nearestPoints, pathData.Points, pointIndex);
                    if (colinearPathIds != null && colinearPathIds.Count > 0)
                    {
                        if (pointIndex > 0)
                        {
                            var prevPoint = pathData.Points[pointIndex - 1];

                            if (prevPoint != null)
                            {
                                pointToNeighborPathId.AddRange(prevPoint, colinearPathIds);
                            }
                        }

                        pathData.Points[pointIndex] = null;
                    }
                    else if (prevColinearPathIds != null)
                    {
                        prevColinearPathIds.ForEach(id => pointToNeighborPathId.Add(point, id));
                    }

                    prevColinearPathIds = colinearPathIds;

                    ++pointIndex;
                }

                pathsList[pathIndex] = null;

                var splitPoints = PointUtils.SplitPoints(pathData.Points);

                if (splitPoints.Count > 0)
                {
                    var newPathsList = new List<PathData>();

                    var index = 0;
                    foreach (var points in splitPoints)
                    {
                        // Keep id for first path data so it's easier to track it
                        PathData newPath = new PathData((index++ == 0) ? pathData.Id : new int?());
                        newPath.Points = points;

                        FillConnections(pointToNeighborPathId, newPath, pathConnections);

                        newPathsList.Add(newPath);
                    }

                    foreach (var path in newPathsList)
                    {
                        mapper.PutPoints(PathPoint.CreateFromPathData(path));
                        newPathsListTotal.Add(path);
                    }
                }

                ++pathIndex;
            }

            pathsList.RemoveAll(p => p == null);
            pathsList.AddRange(newPathsListTotal);
        }

        public static void FillConnections(MultiValueDictionary<GpsCoordinate, int> mapping, PathData pathData, PathConnections connections)
        {
            if (connections == null)
                return;

            // Process path end
            IEnumerable<int> pathIds;
            if (mapping.TryGetValues(pathData.Points.Last(), out pathIds))
            {
                foreach (var pathId in pathIds)
                {
                    connections.Add(pathData.Id, pathId);
                }
            }

            if (mapping.TryGetValues(pathData.Points.First(), out pathIds))
            {
                foreach (var pathId in pathIds)
                {
                    connections.Add(pathId, pathData.Id);
                }
            }
        }

        public static bool AtLeastOneNeighborIsColinear(IEnumerable<PathPoint> nearestPoints, List<GpsCoordinate> points, int pointIndex)
        {
            var colinearPathIds = GetColinearPathIds(nearestPoints, points, pointIndex);

            if (colinearPathIds == null)
                return false;

            return colinearPathIds.Count > 0;
        }

        public static List<int> GetColinearPathIds(IEnumerable<PathPoint> nearestPoints, List<GpsCoordinate> points, int pointIndex)
        {
            if (nearestPoints == null)
                return null;

            var colinearPathIds = new List<int>();

            foreach (var nearestPathPoint in nearestPoints)
            {
                GpsCoordinate nearestPathVector;

                var nextNearestPathPoint = nearestPathPoint.Next;
                var prevNearestPathPoint = nearestPathPoint.Prev;
                if (nextNearestPathPoint != null)
                {
                    nearestPathVector = PointUtils.GetDirection(nearestPathPoint.Point, nextNearestPathPoint.Point);
                }
                else if (prevNearestPathPoint != null)
                {
                    nearestPathVector = PointUtils.GetDirection(prevNearestPathPoint.Point, nearestPathPoint.Point);
                }
                else
                    continue;

                bool prevSectionIsColinear = pointIndex > 0 && points[pointIndex - 1] != null
                    ? PointUtils.CheckColinear(
                        PointUtils.GetDirection(points[pointIndex - 1], points[pointIndex]),
                        nearestPathVector,
                        20)
                    : true;

                bool nextSectionIsColinear = pointIndex < points.Count - 1
                    ? PointUtils.CheckColinear(
                        PointUtils.GetDirection(points[pointIndex], points[pointIndex + 1]),
                        nearestPathVector,
                        20)
                    : true;

                if (prevSectionIsColinear && nextSectionIsColinear)
                {
                    colinearPathIds.Add(nearestPathPoint.PathId);
                }
            }

            return colinearPathIds;
        }
    }
}