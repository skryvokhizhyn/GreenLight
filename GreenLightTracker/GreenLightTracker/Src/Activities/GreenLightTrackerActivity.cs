﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Content;
using Android.Graphics;

using System;

using NLog;

namespace GreenLightTracker.Src.Activities
{
    [Activity(Label = "Green Light Tracker", MainLauncher = true)]
    public class GreenLightTrackerActivity : Activity
    {
        GpsLocationManager m_locationManager;
        GpsLocationBuffer m_locationBuffer = new GpsLocationBuffer();
        PermissionsManager m_permissionsManager = null;
        EventsConnectionManager m_eventConnectionManager = null;
        int m_rowsCount = 0;
        PathView m_pathView = null;
        WakeLockWrapper m_wakeLock = null;
        StorageInterface m_storage = null;
 
        private static Logger m_logger = LogManager.GetCurrentClassLogger();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.tracker_main_widget);

            m_permissionsManager = new PermissionsManager(this);
            m_permissionsManager.RequestPermissions();

            m_locationManager = new GpsLocationManager(this);

            m_eventConnectionManager = new EventsConnectionManager(m_locationManager, m_locationBuffer, this);

            m_pathView = FindViewById<PathView>(Resource.Id.path_view);

            m_wakeLock = new WakeLockWrapper(this);
        }

        protected override void OnDestroy()
        {
            m_locationManager.Stop();
            m_storage.Close();

            base.OnDestroy();
        }

        protected override void OnPause()
        {
            OnStopButtonClick(null);

            base.OnPause();
        }

        protected override void OnPostResume()
        {
            base.OnPostResume();
        }

        protected override void OnStart()
        {
            base.OnStart();
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

            m_locationBuffer.Session = Guid.NewGuid();
            m_locationBuffer.Storage = m_storage;

            m_locationManager.Start();

            FindViewById<Button>(Resource.Id.collect_button).Enabled = false;
            FindViewById<Button>(Resource.Id.stop_button).Enabled = true;
            FindViewById<Button>(Resource.Id.reset_button).Enabled = true;

            m_wakeLock.Acquire();

            FindViewById<RadioButton>(Resource.Id.store_aws_button).Enabled = false;
            FindViewById<RadioButton>(Resource.Id.store_db_button).Enabled = false;
        }

        [Java.Interop.Export()]
        public void OnStopButtonClick(View v)
        {
            m_locationManager.Stop();

            m_eventConnectionManager.Disconnect();

            m_locationBuffer.Flush();

            FindViewById<Button>(Resource.Id.stop_button).Enabled = false;
            FindViewById<Button>(Resource.Id.collect_button).Enabled = true;
            FindViewById<Button>(Resource.Id.track_button).Enabled = true;
            FindViewById<Button>(Resource.Id.reset_button).Enabled = false;

            m_wakeLock.Release();

            FindViewById<RadioButton>(Resource.Id.store_aws_button).Enabled = true;
            FindViewById<RadioButton>(Resource.Id.store_db_button).Enabled = true;
        }

        [Java.Interop.Export()]
        public void OnDrawButtonClick(View v)
        {
            ActivityCallbackUtils.ProcessDrawButtonClick(m_storage, m_pathView);

            FindViewById<Button>(Resource.Id.move_reset).Enabled = true;
            FindViewById<Button>(Resource.Id.move_down).Enabled = true;
            FindViewById<Button>(Resource.Id.move_up).Enabled = true;
            FindViewById<Button>(Resource.Id.move_left).Enabled = true;
            FindViewById<Button>(Resource.Id.move_right).Enabled = true;
            FindViewById<Button>(Resource.Id.zoom_in).Enabled = true;
            FindViewById<Button>(Resource.Id.zoom_out).Enabled = true;
        }

        [Java.Interop.Export()]
        public void OnTrackButtonClick(View v)
        {
            ActivityCallbackUtils.ProcessTrackButtonClick(m_storage, m_eventConnectionManager, m_locationManager, m_pathView);
             
            FindViewById<Button>(Resource.Id.track_button).Enabled = false;
            FindViewById<Button>(Resource.Id.stop_button).Enabled = true;

            m_wakeLock.Acquire();
        }

        [Java.Interop.Export()]
        public void OnResetBufferButtonClick(View v)
        {
            m_locationBuffer.Clear();
        }

        #region View manipulation callbacks

        [Java.Interop.Export()]
        public void OnResetButtonClick(View v)
        {
            m_pathView.ResetZoomAndMove();
        }
        [Java.Interop.Export()]
        public void OnZoomInButtonClick(View v)
        {
            m_pathView.ZoomIn();
        }
        [Java.Interop.Export()]
        public void OnZoomOutButtonClick(View v)
        {
            m_pathView.ZoomOut();
        }
        [Java.Interop.Export()]
        public void OnMoveLeftButtonClick(View v)
        {
            m_pathView.MoveLeft();
        }
        [Java.Interop.Export()]
        public void OnMoveRightButtonClick(View v)
        {
            m_pathView.MoveRight();
        }
        [Java.Interop.Export()]
        public void OnMoveUpButtonClick(View v)
        {
            m_pathView.MoveUp();
        }
        [Java.Interop.Export()]
        public void OnMoveDownButtonClick(View v)
        {
            m_pathView.MoveDown();
        }

        [Java.Interop.Export()]
        public void OnStorageDBClick(View v)
        {
            m_storage?.Close();
            m_storage = new StorageLocalDB();
        }
        [Java.Interop.Export()]
        public void OnStorageAWSClick(View v)
        {
            m_storage?.Close();
            m_storage = new StorageAWS();
        }

        #endregion

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

            if (FindViewById<RadioButton>(Resource.Id.store_db_button).Checked)
            {
                OnStorageDBClick(null);
            }
            else
            {
                OnStorageAWSClick(null);
            }

            FindViewById<Button>(Resource.Id.collect_button).Enabled = true;
            FindViewById<Button>(Resource.Id.track_button).Enabled = true;
            FindViewById<Button>(Resource.Id.draw_button).Enabled = true;
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

        public void OnCarMoved(GpsCoordinate position)
        {
            m_pathView.SetCarPosition(position);
            m_pathView.Invalidate();
        }

        public void OnRoadFound(int pathId)
        {
        }

        public void OnNeighborsUpdated(int count)
        {
            if (count == -1)
            {
                FindViewById<TextView>(Resource.Id.row_neighbors_count).SetBackgroundColor(new Color(Color.Moccasin));
            }
            else
            {
                FindViewById<TextView>(Resource.Id.row_neighbors_count).SetBackgroundColor(new Color(Color.White));
            }

            FindViewById<TextView>(Resource.Id.row_neighbors_count).Text = count.ToString();
        }
    }
}
