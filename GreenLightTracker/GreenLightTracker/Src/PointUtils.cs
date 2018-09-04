using System;
using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class PointUtils
    {
        public static float GetDistance(GpsCoordinate l, GpsCoordinate r)
        {
            return (float)Math.Sqrt(Math.Pow(l.x - r.x, 2) + Math.Pow(l.y - r.y, 2) + Math.Pow(l.z - r.z, 2));
        }

        public static GpsCoordinate GetDirection(GpsCoordinate from, GpsCoordinate to)
        {
            return new GpsCoordinate() { x = to.x - from.x, y = to.y - from.y, z = to.z - from.z };
        }

        public static double GetAngleBetween(GpsCoordinate v1, GpsCoordinate v2)
        {
            var len1 = Math.Sqrt(Math.Pow(v1.x, 2) + Math.Pow(v1.y, 2));
            var len2 = Math.Sqrt(Math.Pow(v2.x, 2) + Math.Pow(v2.y, 2));

            if (len1 == 0.0 || len2 == 0.0)
                return Double.NaN;

            var x1 = v1.x / len1;
            var x2 = v2.x / len2;
            var y1 = v1.y / len1;
            var y2 = v2.y / len2;

            var angleV1 = Math.Atan2(y1, x1);
            var angleV2 = Math.Atan2(y2, x2);

            if (Double.IsNaN(angleV1) || Double.IsNaN(angleV2))
                return Double.NaN;

            return (angleV2 - angleV1) * 180.0 / Math.PI;
        }

        public static bool CheckColinear(GpsCoordinate v1, GpsCoordinate v2, float tolerance)
        {
            return CheckAngleIsBetween(v1, v2, 0, tolerance);
        }

        public static bool CheckAngleIsBetween(GpsCoordinate v1, GpsCoordinate v2, float angleFrom, float angleTo)
        {
            var angleBetween = GetAngleBetween(v1, v2);

            return Math.Abs(angleBetween) >= angleFrom && Math.Abs(angleBetween) <= angleTo;
        }

        public static void EnrichWithIntemediatePoints(ICollection<PathData> paths, float pointsStep)
        {
            if (paths == null)
                return;

            var pathList = (List<PathData>)paths;

            for (var i = 0; i < pathList.Count; ++i)
            {
                GpsCoordinate prevPoint = null;

                var enrichedPoints = new List<GpsCoordinate>();

                var pathData = pathList[i];

                foreach (var currentPoint in pathData.Points)
                {
                    if (enrichedPoints.Count > 0)
                    {
                        prevPoint = enrichedPoints[enrichedPoints.Count - 1];
                    }

                    if (prevPoint != null)
                    {
                        var dist = PointUtils.GetDistance(prevPoint, currentPoint);

                        var direction = new GpsCoordinate
                        {
                            x = currentPoint.x - prevPoint.x,
                            y = currentPoint.y - prevPoint.y,
                            z = currentPoint.z - prevPoint.z
                        };

                        if (dist > 0)
                        {
                            var lenMultiplier = pointsStep / dist;

                            direction.x *= lenMultiplier;
                            direction.y *= lenMultiplier;
                            direction.z *= lenMultiplier;
                        }

                        var incrementalPoint = prevPoint;

                        while (dist > pointsStep)
                        {
                            var point = new GpsCoordinate
                            {
                                x = incrementalPoint.x + direction.x,
                                y = incrementalPoint.y + direction.y,
                                z = incrementalPoint.z + direction.z
                            };

                            enrichedPoints.Add(point);

                            incrementalPoint = point;

                            dist -= pointsStep;
                        }

                        enrichedPoints.Add(currentPoint);
                    }
                    else
                    {
                        enrichedPoints.Add(currentPoint);
                    }
                }

                pathData.Points = enrichedPoints;
            }
        }

        public static int GetLength(PathPoint pathBegin)
        {
            int res = 0;
            var pathPoint = pathBegin;

            while (pathPoint != null)
            {
                ++res;
                pathPoint = pathPoint.Next;
            }

            return res;
        }

        public static PathPoint GetPointOnPath(PathPoint pathBegin, int pos)
        {
            if (GetLength(pathBegin) <= pos)
            {
                throw new ArgumentOutOfRangeException();
            }

            var pathPoint = pathBegin;

            while (--pos >= 0)
            {
                pathPoint = pathPoint.Next;
            }

            return pathPoint;
        }

        public static PathPoint GetLastPoint(PathPoint pathBegin)
        {
            if (pathBegin == null)
                return null;

            var ret = pathBegin;

            while (ret.Next != null)
            {
                ret = ret.Next;
            }

            return ret;
        }

        public static List<PathData> CreateFromPoints(ICollection<GpsCoordinate> points, float tolerance)
        {
            var result = new List<PathData>();
            GpsCoordinate prev = null;

            foreach (var point in points)
            {
                if (result.Count == 0
                    || (prev != null && PointUtils.GetDistance(prev, point) > tolerance))
                {
                    result.Add(new PathData());
                }

                result[result.Count - 1].Points.Add(point);

                prev = point;
            }

            return result;
        }

        public static List<List<GpsCoordinate>> SplitPoints(IEnumerable<GpsCoordinate> points)
        {
            if (points == null)
                return null;

            var res = new List<List<GpsCoordinate>>();

            foreach (var p in points)
            {
                if (res.Count == 0)
                {
                    res.Add(new List<GpsCoordinate>());
                }

                if (p == null)
                {
                    if (res[res.Count - 1].Count > 0)
                    {
                        res.Add(new List<GpsCoordinate>());
                    }
                }
                else
                {
                    res[res.Count - 1].Add(p);
                }
            }

            return res;
        }

        public static List<GpsCoordinate> PathDataToGpsCoordinates(ICollection<PathData> paths)
        {
            var points = new List<GpsCoordinate>();

            foreach(var pathData in paths)
            {
                points.AddRange(pathData.Points);
            }

            return points;
        }

        public static int GetPointsCount(ICollection<PathData> paths)
        {
            if (paths == null)
                return 0;

            var cnt = 0;

            foreach(var path in paths)
            {
                cnt += path.Points.Count;
            }

            return cnt;
        }

        public static ValueTuple<double, double, double, double> GetXYMinMax(ICollection<PathData> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("[GetXYMinMax(ICollection<PathData> paths)] paths is null");

            var xMin = Double.MaxValue;
            var yMin = Double.MaxValue;
            var xMax = Double.MinValue;
            var yMax = Double.MinValue;

            foreach (var path in paths)
            {
                foreach (var p in path.Points)
                {
                    xMin = Math.Min(xMin, p.x);
                    yMin = Math.Min(yMin, p.y);

                    xMax = Math.Max(xMax, p.x);
                    yMax = Math.Max(yMax, p.y);
                }
            }

            return ( xMin, yMin, xMax, yMax );
        }

        public static double GetDistance(IEnumerable<GpsCoordinate> points)
        {
            var length = 0.0;
            GpsCoordinate prevPoint = null;

            foreach (var p in points)
            {
                if (prevPoint != null)
                {
                    length += PointUtils.GetDistance(prevPoint, p);
                }

                prevPoint = p;
            }

            return length;
        }

        public static void RemoveShortPaths(ICollection<PathData> paths, float tolerance = 10)
        {
            if (paths == null)
                return;

            ((List<PathData>)paths).RemoveAll(path => PointUtils.GetDistance(path.Points) < tolerance);
        }

        public static void RemoveLongPaths(ICollection<PathData> paths, float tolerance = 10)
        {
            if (paths == null)
                return;

            ((List<PathData>)paths).RemoveAll(path => PointUtils.GetDistance(path.Points) >= tolerance);
        }

        public static PathData SplitPathDataAtIndex(PathData pathData, int index)
        {
            if (pathData == null || index < 0 || index >= pathData.Points.Count)
                return null;

            var tailPointsCount = pathData.Points.Count - index;

            var tail = new PathData();
            tail.Points = pathData.Points.GetRange(index, tailPointsCount);
            pathData.Points.RemoveRange(index, tailPointsCount);

            return tail;
        }
    }
}