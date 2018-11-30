using System.Collections.Generic;
using System.Linq;

namespace GreenLightTracker.Src
{
    public class RoadSplitter
    {
        private readonly float m_tolerance;
        private readonly PathConnections m_connections;

        public RoadSplitter(float tolerance, PathConnections connections)
        {
            m_tolerance = tolerance;
            m_connections = connections;
        }

        class InOutPaths
        {
            public HashSet<int> inPaths = new HashSet<int>();
            public HashSet<int> outPaths = new HashSet<int>();
        }

        public void Process(ICollection<PathData> paths)
        {
            if (paths == null)
                return;

            var mapper = new PathMapper(m_tolerance);

            foreach (var path in paths)
            {
                mapper.PutPoints(PathPoint.CreateFromPathData(path));
            }

            var pointToPath = new Dictionary<GpsCoordinate, InOutPaths>();
            var directConnections = new PathConnections();

            foreach (var path in paths)
            {
                {
                    var firstPoint = path.Points.First();

                    var neighbors = mapper.GetNearestPointsFiltered(firstPoint);
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.PathId == path.Id)
                            continue;

                        if (m_connections != null && !m_connections.HasConnection(neighbor.PathId, path.Id))
                            continue;

                        // Last point to First point
                        if (neighbor.Next != null)
                        {
                            var inOutVal = SafeGetValue(pointToPath, neighbor.Point);
                            inOutVal.outPaths.Add(path.Id);
                        }
                    }
                }

                {
                    var lastPoint = path.Points.Last();

                    var neighbors = mapper.GetNearestPointsFiltered(lastPoint);
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.PathId == path.Id)
                            continue;

                        if (m_connections != null && !m_connections.HasConnection(path.Id, neighbor.PathId))
                            continue;

                        if (neighbor.Prev == null)
                        {
                            directConnections.Add(path.Id, neighbor.PathId);
                        }
                        else
                        {
                            var inOutVal = SafeGetValue(pointToPath, neighbor.Point);
                            inOutVal.inPaths.Add(path.Id);
                        }
                    }
                }
            }

            m_connections?.Clear();

            for (var pathIndex = 0; pathIndex < paths.Count; ++pathIndex)
            {
                var path = paths.ElementAt(pathIndex);
                var points = path.Points;

                if (m_connections != null)
                {
                    var directPaths = directConnections.GetConnections(path.Id);
                    foreach (var pathId in directPaths)
                    {
                        m_connections.Add(path.Id, pathId);
                        directConnections.Remove(pathId, path.Id);
                    }
                }

                for (var pointIndex = 1; pointIndex < points.Count - 1; ++pointIndex)
                {
                    InOutPaths inOutPaths;
                    if (pointToPath.TryGetValue(points[pointIndex], out inOutPaths))
                    {
                        var tail = PointUtils.SplitPathDataAtIndex(path, pointIndex);

                        if (tail == null)
                            continue;

                        if (m_connections != null)
                        {
                            PutConnections(path.Id, tail.Id, inOutPaths.inPaths, true);
                            PutConnections(path.Id, tail.Id, inOutPaths.outPaths, false);
                        }

                        paths.Add(tail);
                    }
                }

                break;
            }
        }

        TValue SafeGetValue<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key) where TValue: class, new()
        {
            TValue val;
            if (!dict.TryGetValue(key, out val))
            {
                val = new TValue();
                dict.Add(key, val);
            }

            return val;
        }

        void PutConnections(int pathId, int tailId, HashSet<int> ids, bool isInPaths)
        {
            bool firstProcessed = false;

            foreach (var id in ids)
            {
                if (!firstProcessed)
                    m_connections.Split(pathId, tailId, id, isInPaths);
                else
                    m_connections.Add(pathId, id);

                firstProcessed = true;
            }
        }
    }
}