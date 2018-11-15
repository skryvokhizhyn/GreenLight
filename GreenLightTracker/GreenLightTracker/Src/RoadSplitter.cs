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

            foreach (var path in paths)
            {
                {
                    var firstPoint = path.Points.First();

                    var neighbors = mapper.GetNearestPointsFiltered(firstPoint);
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.PathId == path.Id)
                            continue;

                        var inOutVal = SafeGetValue(pointToPath, neighbor.Point);
                        inOutVal.outPaths.Add(path.Id);
                    }
                }

                {
                    var lastPoint = path.Points.Last();

                    var neighbors = mapper.GetNearestPointsFiltered(lastPoint);
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.PathId == path.Id)
                            continue;

                        var inOutVal = SafeGetValue(pointToPath, neighbor.Point);
                        inOutVal.inPaths.Add(path.Id);
                    }
                }
            }

            for (var pathIndex = 0; pathIndex < paths.Count; ++pathIndex)
            {
                var path = paths.ElementAt(pathIndex);
                var points = path.Points;

                for (var pointIndex = 1; pointIndex < points.Count - 1; ++pointIndex)
                {
                    InOutPaths inOutPaths;
                    if (pointToPath.TryGetValue(points[pointIndex], out inOutPaths))
                    {
                        var tail = PointUtils.SplitPathDataAtIndex(path, pointIndex);

                        if (tail == null)
                            continue;

                        {
                            bool firstProcessed = false;

                            foreach (var inPathId in inOutPaths.inPaths)
                            {
                                if (firstProcessed)
                                    m_connections?.Split(path.Id, tail.Id, inPathId, true);
                                else
                                    m_connections?.Add(inPathId, tail.Id);

                                firstProcessed = true;
                            }
                        }

                        {
                            bool firstProcessed = false;

                            foreach (var outPathId in inOutPaths.outPaths)
                            {
                                if (firstProcessed)
                                    m_connections?.Split(path.Id, tail.Id, outPathId, false);
                                else
                                    m_connections?.Add(path.Id, outPathId);

                                firstProcessed = true;
                            }
                        }

                        paths.Add(tail);
                    }
                }
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
    }
}