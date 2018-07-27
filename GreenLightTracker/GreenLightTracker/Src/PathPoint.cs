using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class PathPoint
    {
        public GpsCoordinate Point { get; set; }
        public int PathId { get; set; }

        public PathPoint Next { get; private set; }
        public PathPoint Prev { get; private set; }

        private static void AddAfter(PathPoint anchorPoint, PathPoint newPoint)
        {
            var nextPoint = anchorPoint.Next;

            anchorPoint.Next = newPoint;
            newPoint.Prev = anchorPoint;

            if (nextPoint != null)
            {
                nextPoint.Prev = newPoint;
                newPoint.Next = nextPoint;
            }
        }

        public static PathPoint CreateFromPathData(PathData pathData)
        {
            PathPoint result = null;
            PathPoint prev = null;

            foreach (var point in pathData.Points)
            {
                if (point == null)
                    continue;

                var pathPoint = new PathPoint() { Point = point, PathId = pathData.Id };

                if (result == null)
                {
                    result = pathPoint;
                }
                else
                {
                    PathPoint.AddAfter(prev, pathPoint);
                }

                prev = pathPoint;
            }

            return result;
        }

        public static PathPoint CreateFromPoints(ICollection<GpsCoordinate> points, int pathId)
        {
            PathPoint result = null;
            PathPoint prev = null;

            foreach (var point in points)
            {
                var pathPoint = new PathPoint() { Point = point, PathId = pathId };

                if (result == null)
                {
                    result = pathPoint;
                }
                else
                {
                    PathPoint.AddAfter(prev, pathPoint);
                }

                prev = pathPoint;
            }

            return result;
        }

        //private static PathPoint AddAfter(PathPoint anchorPoint, GpsCoordinate point)
        //{
        //    var nextPoint = anchorPoint.Next;

        //    var newPoint = new PathPoint
        //    {
        //        Point = point,
        //        PathId = anchorPoint.PathId,
        //    };

        //    anchorPoint.Next = newPoint;
        //    newPoint.Prev = anchorPoint;

        //    if (nextPoint != null)
        //    {
        //        nextPoint.Prev = newPoint;
        //        newPoint.Next = nextPoint;
        //    }

        //    return newPoint;
        //}

        //public static void AddBerofe(PathPoint anchorPoint, PathPoint newPoint)
        //{
        //    var prevPoint = anchorPoint.Prev;

        //    anchorPoint.Prev = newPoint;
        //    newPoint.Next = anchorPoint;

        //    if (prevPoint != null)
        //    {
        //        prevPoint.Next = newPoint;
        //        newPoint.Prev = prevPoint;
        //    }
        //}

        //public static void Remove(PathPoint point)
        //{
        //    if (point.Prev != null)
        //    {
        //        point.Prev.Next = point.Next;
        //    }

        //    if (point.Next != null)
        //    {
        //        point.Next.Prev = point.Prev;
        //    }

        //    point.Next = null;
        //    point.Prev = null;
        //}

        //public static void TrimBefore(PathPoint point)
        //{
        //    if (point == null)
        //        return;

        //    if (point.Prev != null)
        //    {
        //        point.Prev.Next = null;
        //    }

        //    point.Prev = null;
        //}
    }
}