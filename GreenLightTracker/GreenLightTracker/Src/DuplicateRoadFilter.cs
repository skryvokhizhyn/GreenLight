using System;
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

        //public ICollection<GpsCoordinate> Process(ICollection<GpsCoordinate> points)
        //{
        //    var splitter = new RoadSplitter(points, m_pointsTolerance);
        //    var mapper = new PathMapper();

        //    while (true)
        //    {
        //        var path = splitter.Next();

        //        if (path == null)
        //            break;

        //        var pathList = (List<GpsCoordinate>)path;
        //        var i = 0;

        //        while (i < pathList.Count)
        //        {
        //            IList<PathPoint> neighbors = null;

        //            var tempPoints = GetPointsTillNeighbor(i, pathList, mapper, out neighbors);

        //            i += tempPoints.Count;

        //            //mapper.PutPoints(tempPoints);

        //            if (neighbors == null ||i == pathList.Count)
        //            {
        //                continue;
        //            }

        //            i = SkipPointsOnPath(i, pathList, neighbors);
        //        }
        //    }

        //    return mapper.GetPoints();
        //}

        //private List<GpsCoordinate> GetPointsTillNeighbor(int i, List<GpsCoordinate> pathList, PathMapper mapper,
        //    out IList<PathPoint> neighbors)
        //{
        //    var tempPoints = new List<GpsCoordinate>();
        //    neighbors = null;

        //    while (i < pathList.Count)
        //    {
        //        var p = pathList[i];

        //        neighbors = mapper.GetNearestPointsFiltered(p);

        //        if (neighbors != null)
        //        {
        //            break;
        //        }

        //        tempPoints.Add(p);

        //        ++i;
        //    }

        //    if (i == pathList.Count)
        //    {
        //        return tempPoints;
        //    }

        //    var prevPoint = pathList[i];

        //    ++i;

        //    // Process points between last and neighbor
        //    while (i < pathList.Count)
        //    {
        //        var p = pathList[i];

        //        var hasIntersection = false;
        //        var dist1 = PointUtils.GetDistance(prevPoint, p);

        //        foreach (var n in neighbors)
        //        {
        //            var dist2 = PointUtils.GetDistance(prevPoint, n.Point);

        //            if (dist1 > dist2)
        //            {
        //                hasIntersection = true;
        //                break;
        //            }
        //        }

        //        if (hasIntersection)
        //        {
        //            tempPoints.Add(p);
        //            break;
        //        }

        //        tempPoints.Add(prevPoint);

        //        prevPoint = p;

        //        ++i;
        //    }

        //    return tempPoints;
        //}

        //private int SkipPointsOnPath(int i, List<GpsCoordinate> pathList, IList<PathPoint> neighbors)
        //{
        //    while (i < pathList.Count)
        //    {
        //        var p = pathList[i];

        //        AdvanceNeighbors(p, neighbors);
        //        RemoveNeighborsWithoutNext(neighbors);

        //        if (!IsOnPath(p, neighbors))
        //            break;

        //        ++i;
        //    }

        //    return i;
        //}

        //private static void AdvanceNeighbors(GpsCoordinate point, IList<PathPoint> neighbors)
        //{
        //    var neighborsList = (List<PathPoint>)neighbors;

        //    for (var i = 0; i< neighborsList.Count; ++i)
        //    {
        //        var n = neighborsList[i];

        //        while (n.Next != null)
        //        {
        //            var dist1 = PointUtils.GetDistance(n.Point, point);
        //            var dist2 = PointUtils.GetDistance(n.Point, n.Next.Point);

        //            if (dist1 >= dist2)
        //            {
        //                n = n.Next;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }
        //}

        //private static void RemoveNeighborsWithoutNext(IList<PathPoint> neighbors)
        //{
        //    var neighborsList = (List<PathPoint>)neighbors;

        //    neighborsList.RemoveAll(n => { return n.Next == null; });
        //}

        //private static bool IsOnPath(GpsCoordinate point, IList<PathPoint> neighbors)
        //{
        //    var isOnPath = false;

        //    foreach (var n in neighbors)
        //    {
        //        var dist1 = PointUtils.GetDistance(n.Point, point);
        //        var dist2 = PointUtils.GetDistance(n.Point, n.Next.Point);

        //        if (dist1 <= dist2)
        //        {
        //            isOnPath = true;
        //            break;
        //        }
        //    }

        //    return isOnPath;
        //}

        public void Process(ICollection<PathData> paths)
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

                while (pointIndex < pathData.Points.Count)
                {
                    var point = pathData.Points[pointIndex];

                    if (mapper.GetNearestPointsFiltered(point) != null)
                    {
                        pathData.Points[pointIndex] = null;
                    }

                    ++pointIndex;
                }

                var splitPoints = PointUtils.SplitPoints(pathData.Points);

                if (splitPoints.Count > 1)
                {
                    var newPathsList = new List<PathData>();

                    foreach (var points in splitPoints)
                    {
                        var newPath = new PathData();
                        newPath.Points = points;

                        newPathsList.Add(newPath);
                    }

                    pathsList[pathIndex] = null;

                    foreach (var path in newPathsList)
                    {
                        mapper.PutPoints(PathPoint.CreateFromPathData(path));
                        newPathsListTotal.Add(path);
                    }
                }
                else if (splitPoints.Count == 1)
                {
                    if (splitPoints[0].Count == 0)
                    {
                        pathsList[pathIndex] = null;
                    }
                    else
                    {
                        pathData.Points.RemoveAll(p => p == null);
                        mapper.PutPoints(PathPoint.CreateFromPathData(pathData));
                    }
                }
                else
                {
                    pathsList[pathIndex] = null;
                }
                
                ++pathIndex;
            }

            pathsList.RemoveAll(p => p == null);
            pathsList.AddRange(newPathsListTotal);
        }
    }
}