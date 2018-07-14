using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Content;

using NLog;

namespace GreenLightTracker.Src
{
    [Activity(Label = "Green Light Tracker", MainLauncher = true)]
    public class GreenLightTrackerActivity : Activity
    {
        const int LocationRequestId = 1324657;
        const int ExternalStorageRequestId = 43243;
        GpsLocationManager m_locationManager;
        DBQueryManager m_queryManager = new DBQueryManager();
        int m_rowsCount = 0;
        PathView m_pathView = null;

        private static Logger m_logger = LogManager.GetCurrentClassLogger();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.tracker_main_widget);

            requestAccessLocationPermissions();
            requestExternalStorageAccessPermissions();

            m_locationManager = new GpsLocationManager(this);

            //FindViewById<TextView>(Resource.Id.db_path_text).Text = m_queryManager.PathToDB;

            m_pathView = FindViewById<PathView>(Resource.Id.path_view);
        }

        protected override void OnDestroy()
        {
            m_locationManager.Stop();
            m_queryManager.Close();

            base.OnDestroy();
        }

        // View actions
        [Java.Interop.Export()]
        public void OnExitButtonClick(View v)
        {
            Finish();
        }

        [Java.Interop.Export()]
        public void OnCollectButtonClick(View v)
        {
            m_locationManager.GpsLocationReceived += m_queryManager.OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived += OnGpsLocationReceived;

            m_locationManager.Start();
            FindViewById<Button>(Resource.Id.collect_button).Enabled = false;
            FindViewById<Button>(Resource.Id.stop_button).Enabled = true;
        }

        [Java.Interop.Export()]
        public void OnStopButtonClick(View v)
        {
            m_locationManager.Stop();

            m_locationManager.GpsLocationReceived -= OnGpsLocationReceived;
            m_locationManager.GpsLocationReceived -= m_queryManager.OnGpsLocationReceived;

            FindViewById<Button>(Resource.Id.stop_button).Enabled = false;
            FindViewById<Button>(Resource.Id.collect_button).Enabled = true;
        }

        //[Java.Interop.Export()]
        //public void DBPageButtonClick(View v)
        //{
        //    StartActivity(typeof(DBPageActivity));
        //}

        [Java.Interop.Export()]
        public void OnRowCountButtonClick(View v)
        {
            var cnt = m_queryManager.GetGpsLocationCount();
            FindViewById<TextView>(Resource.Id.row_count_total_text).Text = cnt.ToString();
            m_rowsCount = 0;
            UpdateTakenRowCount();
        }

        [Java.Interop.Export()]
        public void OnDBBackupButtonClick(View v)
        {
            m_locationManager.Stop();
            m_queryManager.Close();

            var backupFolder = System.IO.Path.Combine(
                Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath,
                "GreenLightTracker");

            var backupPath = m_queryManager.CreateBackup(backupFolder);
            Toast.MakeText(this, $"Backup location is: {backupPath}", ToastLength.Long).Show();

            m_queryManager.Open();
            m_locationManager.Start();
        }

        [Java.Interop.Export()]
        public void OnDBCleanupButtonClick(View v)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder
                .SetMessage("Do you want to clean up DB?")
                .SetTitle("DB clean up")
                .SetPositiveButton("Ok", (object sender, DialogClickEventArgs e) =>
                    {
                        m_queryManager.CleanGpsLocationTable();
                        Toast.MakeText(this, "GpsLocation table cleaned up.", ToastLength.Long).Show();
                    })
                .SetNegativeButton("No", (object sender, DialogClickEventArgs e) => { });
            builder.Create().Show();
        }

        [Java.Interop.Export()]
        public void OnDrawButtonClick(View v)
        {
            var locations = m_queryManager.GetAllGpsLocations();

            var pathPoints = GpsUtils.GetPathPointsFromLocations(locations, 3000 /*ms*/);

            var initialCount = PointUtils.GetPointsCount(pathPoints);

            PointUtils.EnrichWithIntemediatePoints(pathPoints, 2 /*m*/);

            var afterEnrichmentCount = PointUtils.GetPointsCount(pathPoints);

            //FindViewById<TextView>(Resource.Id.row_count_filtered_text).Text = coordinates.Count.ToString();

            //var duplicateFilter = new DuplicateRoadFilter(10);
            //duplicateFilter.Process(pathPoints);

            var afterFilterCount = PointUtils.GetPointsCount(pathPoints);

            var filteredPoints = PointUtils.PathDataToGpsCoordinates(pathPoints);

            m_pathView.SetPoints(filteredPoints);
            m_pathView.Invalidate();
        }

        [Java.Interop.Export()]
        public void OnTrackButtonClick(View v)
        {
            var locations = m_queryManager.GetAllGpsLocations();

            var pathPoints = GpsUtils.GetPathPointsFromLocations(locations, 3000 /*ms*/);

            var initialCount = PointUtils.GetPointsCount(pathPoints);

            PointUtils.EnrichWithIntemediatePoints(pathPoints, 2 /*m*/);

            //m_locationManager.GpsLocationReceived += m_queryManager.OnGpsLocationReceived;

            m_locationManager.Start();
            FindViewById<Button>(Resource.Id.collect_button).Enabled = false;
            FindViewById<Button>(Resource.Id.stop_button).Enabled = true;
        }

        private void ShowTerminateWindow(string text)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder
                .SetMessage("bla")
                .SetTitle(text)
                .SetPositiveButton("Ok", (object sender, DialogClickEventArgs e) => { Finish(); });
            builder.Create().Show();
        }

        private void requestAccessLocationPermissions()
        {
            const string permission = Android.Manifest.Permission.AccessFineLocation;

            if ((int)Build.VERSION.SdkInt < (int)BuildVersionCodes.M
                || CheckSelfPermission(permission) != Permission.Granted)
            {
                string[] PermissionsLocation = { permission };

                m_logger.Trace("Requesting location permissions");

                RequestPermissions(PermissionsLocation, LocationRequestId);
            }
        }

        private void requestExternalStorageAccessPermissions()
        {
            string[] permissions = { Android.Manifest.Permission.ReadExternalStorage, Android.Manifest.Permission.WriteExternalStorage };

            if ((int)Build.VERSION.SdkInt < (int)BuildVersionCodes.M
                || CheckSelfPermission(permissions[0]) != Permission.Granted
                || CheckSelfPermission(permissions[1]) != Permission.Granted)
            {
                m_logger.Trace("Requesting external storage permissions");

                RequestPermissions(permissions, ExternalStorageRequestId);
            }
            else
            {
                m_queryManager.Open();
            }
        }

        public override void OnRequestPermissionsResult(
            int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (permissions.Length == 0)
                return;

            switch (requestCode)
            {
                case LocationRequestId:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            Toast.MakeText(this, "Location permission granted.", ToastLength.Long).Show();
                            m_logger.Info("Location permissions granted.");
                        }
                        else
                        {
                            ShowTerminateWindow("Failed to create location manater. No permissions were granted.");
                        }
                    }
                    break;

                case ExternalStorageRequestId:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            Toast.MakeText(this, "External Storage permission granted.", ToastLength.Long).Show();
                            m_logger.Info("External Storage permissions granted.");
                            m_queryManager.Open();
                        }
                        else
                        {
                            ShowTerminateWindow("Failed to get External Storage permissions. No permissions were granted.");
                        }
                    }
                    break;
            }
        }

        private void UpdateTakenRowCount()
        {
            FindViewById<TextView>(Resource.Id.row_count_taken_text).Text = m_rowsCount.ToString();
        }

        private void OnGpsLocationReceived(GpsLocation l)
        {
            ++m_rowsCount;
            UpdateTakenRowCount();
        }
    }
}
