using Android.App;
using Android.Content;
using Android.Widget;
using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    using PathInformation = System.Tuple<ICollection<PathData>, PathConnections>;

    class ActivityCallbackUtils
    {
        public static void ProcessTrackButtonClick(
            RoadTracker roadTracker, DBQueryManager queryManager, EventsConnectionManager eventConnectionManager, GpsLocationManager locationManager, PathView pathView)
        {
            if (!roadTracker.IsInitialized())
            {
                var pathPoints = GetPathData(queryManager).Item1;

                var pathMapper = new PathMapper(10);
                pathMapper.PutPointList(pathPoints);
                roadTracker.SetMapper(pathMapper);

                var xyMinMax = PointUtils.GetXYMinMax(pathPoints);

                pathView.XMin = xyMinMax.Item1;
                pathView.YMin = xyMinMax.Item2;
                pathView.XMax = xyMinMax.Item3;
                pathView.YMax = xyMinMax.Item4;
            }

            eventConnectionManager.ConnectForTracking();

            locationManager.Start();
        }

        static int a = 0;

        public static void ProcessDrawButtonClick(DBQueryManager queryManager, PathView pathView)
        {
            var pathInformation = GetPathData(queryManager);
            var pathPoints = pathInformation.Item1;
            var pathConnections = pathInformation.Item2;

            var xyMinMax = PointUtils.GetXYMinMax(pathPoints);

            pathView.XMin = xyMinMax.Item1;
            pathView.YMin = xyMinMax.Item2;
            pathView.XMax = xyMinMax.Item3;
            pathView.YMax = xyMinMax.Item4;

            //var gpsCoordinates = PointUtils.PathDataToGpsCoordinates(pathPoints);
            //pathView.SetPoints(gpsCoordinates);

            int i = 0;

            PathData currentPathData = null;
            foreach (var p in pathPoints)
            {
                if (i == a)
                    currentPathData = p;

                ++i;
            }

            int j = 0;

            pathView.ResetColoredPoints();
            foreach (var p in pathPoints)
            {
                if (pathConnections.HasConnection(currentPathData.Id, p.Id))
                {
                    //pathView.AppendColoredPoints(p.Points);
                }
                //67
                //if (j == a)
                if (p.Id == 67)
                {
                    pathView.AppendColoredPoints(p.Points);
                    var d = PointUtils.GetDistance(p.Points);
                }

                ++j;
            }

            ++a;

            pathView.Invalidate();
        }

        public static void ProcessDBCleanupButtonClick(Activity activity, DBQueryManager queryManager)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder
                .SetMessage("Do you want to clean up DB?")
                .SetTitle("DB clean up")
                .SetPositiveButton("Ok", (object sender, DialogClickEventArgs e) =>
                {
                    queryManager.CleanGpsLocationTable();
                    Toast.MakeText(activity, "GpsLocation table cleaned up.", ToastLength.Long).Show();
                })
                .SetNegativeButton("No", (object sender, DialogClickEventArgs e) => { });
            builder.Create().Show();
        }

        private static PathInformation GetPathData(DBQueryManager queryManager)
        {
            var locations = queryManager.GetAllGpsLocations();
            var pathPoints = GpsUtils.GetPathPointsFromLocations(locations, 3000 /*ms*/);

            var initialCount = PointUtils.GetPointsCount(pathPoints);

            PointUtils.RemoveShortPaths(pathPoints, 1000 /*m*/, null);

            var afterFirstRemovalOfShortPaths = PointUtils.GetPointsCount(pathPoints);

            PointUtils.EnrichWithIntemediatePoints(pathPoints, 2 /*m*/);

            var enrichedCount = PointUtils.GetPointsCount(pathPoints);

            var duplicatesFilter = new DuplicateRoadFilter(10 /*m*/);
            var pathConnections = new PathConnections();
            duplicatesFilter.Process(pathPoints, pathConnections);

            var deduplicatedCount = PointUtils.GetPointsCount(pathPoints);

            PointUtils.RemoveShortPaths(pathPoints, 1000 /*m*/, pathConnections);

            var shortenedCount = PointUtils.GetPointsCount(pathPoints);

            var roadSplitter = new RoadSplitter(11 /*m*/, pathConnections);
            roadSplitter.Process(pathPoints);

            // Remove after splitting short artifacts
            PointUtils.RemoveShortPaths(pathPoints, 4 /*m*/, pathConnections);

            return new PathInformation(pathPoints, pathConnections);
        }
    }
}