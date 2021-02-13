using Android.App;
using Android.Content;
using Android.Widget;
using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    using PathInformation = ICollection<PathData>;

    class ActivityCallbackUtils
    {
        public static void ProcessTrackButtonClick(
            StorageInterface storage, EventsConnectionManager eventConnectionManager, GpsLocationManager locationManager, PathView pathView)
        {
            eventConnectionManager.ConnectForTracking();

            locationManager.Start();
        }

        public static void ProcessDrawButtonClick(StorageInterface storage, PathView pathView)
        {
            var pathPoints = GetPathData(storage);

            var xyMinMax = PointUtils.GetXYMinMax(pathPoints);

            pathView.XMin = xyMinMax.Item1;
            pathView.YMin = xyMinMax.Item2;
            pathView.XMax = xyMinMax.Item3;
            pathView.YMax = xyMinMax.Item4;

            var gpsCoordinates = PointUtils.PathDataToGpsCoordinates(pathPoints);
            pathView.SetPoints(gpsCoordinates);

            pathView.Invalidate();
        }

        public static void ProcessDBCleanupButtonClick(Activity activity, StorageLocalDB storage)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder
                .SetMessage("Do you want to clean up DB?")
                .SetTitle("DB clean up")
                .SetPositiveButton("Ok", (object sender, DialogClickEventArgs e) =>
                {
                    storage.CleanGpsLocationTable();
                    Toast.MakeText(activity, "GpsLocation table cleaned up.", ToastLength.Long).Show();
                })
                .SetNegativeButton("No", (object sender, DialogClickEventArgs e) => { });
            builder.Create().Show();
        }

        private static PathInformation GetPathData(StorageInterface storage)
        {
            var locations = storage.GetAllGpsLocations();
            var pathPoints = GpsUtils.GetPathPointsFromLocations(locations, 3000 /*ms*/);

            var initialCount = PointUtils.GetPointsCount(pathPoints);

            PointUtils.RemoveShortPaths(pathPoints, 1000 /*m*/);

            var afterFirstRemovalOfShortPaths = PointUtils.GetPointsCount(pathPoints);

            PointUtils.EnrichWithIntemediatePoints(pathPoints, 2 /*m*/);

            var enrichedCount = PointUtils.GetPointsCount(pathPoints);

            return pathPoints;
        }
    }
}