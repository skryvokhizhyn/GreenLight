using System;
using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public static class GpsUtils
    {
        private const double EARTH_RADIUS_METERS = 6378100.0;

        public static GpsCoordinate GetCoordinateFromLocation(GpsLocation location)
        {
            var latRad = (location.Latitude * Math.PI) / 180.0;
            var lonRad = (location.Longitude * Math.PI) / 180.0;

            var cosLat = Math.Cos(latRad);

            return new GpsCoordinate
            {
                x = EARTH_RADIUS_METERS * cosLat * Math.Cos(lonRad),
                y = EARTH_RADIUS_METERS * cosLat * Math.Sin(lonRad),
                z = EARTH_RADIUS_METERS * Math.Sin(latRad) + location.Altitude
            };
        }

        //public static PathPoint GetPathPointsFromLocations(IEnumerable<GpsLocation> locations, int roadTimestampDifference)
        //{
        //    PathPoint result = null;
        //    int pathId = 0;
        //    int prevTimestamp = -1;
        //    PathPoint prevPathPoint = null;

        //    foreach (var location in locations)
        //    {
        //        var gpsCoordinate = GetCoordinateFromLocation(location);

        //        if (prevTimestamp != -1 && location.Timestamp - prevTimestamp > roadTimestampDifference)
        //            ++pathId;

        //        prevTimestamp = location.Timestamp;

        //        var pathPoint = new PathPoint()
        //        {
        //            Point = gpsCoordinate,
        //            PathId = pathId
        //        };

        //        if (prevPathPoint != null)
        //            PathPoint.AddAfter(prevPathPoint, pathPoint);
        //        else
        //            result = pathPoint;

        //        prevPathPoint = pathPoint;
        //    }

        //    return result;
        //}

        public static ICollection<PathData> GetPathPointsFromLocations(IEnumerable<GpsLocation> locations, int roadTimestampDifference)
        {
            var result = new List<PathData>();
            int prevTimestamp = -1;

            var tempPoints = new List<GpsCoordinate>();

            foreach (var location in locations)
            {
                var gpsCoordinate = GetCoordinateFromLocation(location);

                if (prevTimestamp != -1 && location.Timestamp - prevTimestamp > roadTimestampDifference)
                {
                    var path = new PathData();
                    path.Points = tempPoints;

                    result.Add(path);

                    tempPoints = new List<GpsCoordinate>();
                }

                prevTimestamp = location.Timestamp;

                tempPoints.Add(gpsCoordinate);
            }

            if (tempPoints.Count > 0)
            {
                var path = new PathData();
                path.Points = tempPoints;

                result.Add(path);
            }

            return result;
        }
    }
}