using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GreenLightTracker.Src
{
    class ActivityCallbackUtils
    {
        public static void ProcessTrackButtonClick(
            RoadTracker roadTracker, DBQueryManager queryManager, EventsConnectionManager eventConnectionManager, GpsLocationManager locationManager)
        {
            if (!roadTracker.IsInitialized())
            {
                var locations = queryManager.GetAllGpsLocations();

                var pathPoints = GpsUtils.GetPathPointsFromLocations(locations, 3000 /*ms*/);

                PointUtils.EnrichWithIntemediatePoints(pathPoints, 2 /*m*/);

                var pathMapper = new PathMapper(10);
                pathMapper.PutPointList(pathPoints);
                roadTracker.SetMapper(pathMapper);
            }

            eventConnectionManager.ConnectForTracking();

            locationManager.Start();
        }

        public static void ProcessDrawButtonClick(DBQueryManager queryManager, PathView pathView)
        {
            var locations = queryManager.GetAllGpsLocations();
            var pathPoints = GpsUtils.GetPathPointsFromLocations(locations, 3000 /*ms*/);
            var initialCount = PointUtils.GetPointsCount(pathPoints);

            PointUtils.EnrichWithIntemediatePoints(pathPoints, 2 /*m*/);

            var afterEnrichmentCount = PointUtils.GetPointsCount(pathPoints);

            var filteredPoints = PointUtils.PathDataToGpsCoordinates(pathPoints);

            pathView.SetPoints(null, filteredPoints);
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
    }
}