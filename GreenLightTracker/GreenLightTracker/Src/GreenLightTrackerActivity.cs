using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Content;
using Android.Graphics;

using NLog;

namespace GreenLightTracker.Src
{
    [Activity(Label = "Green Light Tracker", MainLauncher = true)]
    public class GreenLightTrackerActivity : Activity
    {
        GpsLocationManager m_locationManager;
        DBQueryManager m_queryManager = new DBQueryManager();
        PermissionsManager m_permissionsManager = null;
        EventsConnectionManager m_eventConnectionManager = null;
        int m_rowsCount = 0;
        PathView m_pathView = null;
        RoadTracker m_roadTracker = new RoadTracker();
        RoadInformation m_roadInformation = new RoadInformation();

        private static Logger m_logger = LogManager.GetCurrentClassLogger();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.tracker_main_widget);

            m_permissionsManager = new PermissionsManager(this);
            m_permissionsManager.RequestPermissions();

            m_locationManager = new GpsLocationManager(this);

            m_eventConnectionManager = new EventsConnectionManager(m_locationManager, m_queryManager, m_roadTracker, this);

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
            m_eventConnectionManager.ConnectForCollection();

            m_locationManager.Start();

            FindViewById<Button>(Resource.Id.collect_button).Enabled = false;
            FindViewById<Button>(Resource.Id.stop_button).Enabled = true;
        }

        [Java.Interop.Export()]
        public void OnStopButtonClick(View v)
        {
            m_locationManager.Stop();

            m_eventConnectionManager.Disconnect();

            FindViewById<Button>(Resource.Id.stop_button).Enabled = false;
            FindViewById<Button>(Resource.Id.collect_button).Enabled = true;
            FindViewById<Button>(Resource.Id.track_button).Enabled = true;
        }

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

            var backupPath = m_queryManager.CreateBackup();
            Toast.MakeText(this, $"Backup location is: {backupPath}", ToastLength.Long).Show();

            m_queryManager.Open();
            m_locationManager.Start();
        }

        [Java.Interop.Export()]
        public void OnDBCleanupButtonClick(View v)
        {
            ActivityCallbackUtils.ProcessDBCleanupButtonClick(this, m_queryManager);
        }

        [Java.Interop.Export()]
        public void OnDrawButtonClick(View v)
        {
            ActivityCallbackUtils.ProcessDrawButtonClick(m_queryManager, m_pathView);
        }

        [Java.Interop.Export()]
        public void OnTrackButtonClick(View v)
        {
            ActivityCallbackUtils.ProcessTrackButtonClick(m_roadTracker, m_queryManager, m_eventConnectionManager, m_locationManager);

            FindViewById<Button>(Resource.Id.track_button).Enabled = false;
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

        public override void OnRequestPermissionsResult(
            int requestCode, string[] permissions, Permission[] grantResults)
        {
            var result = m_permissionsManager.ProcessPermissionsResult(requestCode, permissions, grantResults);

            if (result != null)
                ShowTerminateWindow("Failed to request permissions: " + result.ToString());
            else
                m_queryManager.Open();
        }

        private void UpdateTakenRowCount()
        {
            FindViewById<TextView>(Resource.Id.row_count_taken_text).Text = m_rowsCount.ToString();
        }

        public void OnGpsLocationReceived(GpsLocation l)
        {
            ++m_rowsCount;
            UpdateTakenRowCount();
        }

        public void OnRoadLightFound(GpsCoordinate position, PathPoint pathPoint, int count)
        {
            if (pathPoint != null)
            {
                FindViewById<TextView>(Resource.Id.row_road_id).Text = pathPoint.PathId.ToString();
                FindViewById<TextView>(Resource.Id.row_road_id).SetBackgroundColor(new Color(Color.Magenta));
                FindViewById<TextView>(Resource.Id.row_neighbors_count).Text = count.ToString();

                m_pathView.SetPoints(position, m_roadTracker.GetPathById(pathPoint.PathId));
                m_pathView.Invalidate();
            }
            else
            {
                FindViewById<TextView>(Resource.Id.row_road_id).Text = "N/A";
                FindViewById<TextView>(Resource.Id.row_road_id).SetBackgroundColor(new Color(Color.White));
                FindViewById<TextView>(Resource.Id.row_neighbors_count).Text = "-1";

                m_pathView.SetPoints(null, null);
                m_pathView.Invalidate();
            }

            FindViewById<TextView>(Resource.Id.row_neighbors_count).SetBackgroundColor(new Color(Color.White));
        }

        public void OnNeighborsLost()
        {
            FindViewById<TextView>(Resource.Id.row_neighbors_count).SetBackgroundColor(new Color(Color.Moccasin));
        }
    }
}
