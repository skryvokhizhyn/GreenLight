using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace GreenLightTracker.Src.Activities
{
    [Activity(Label = "DBPageActivity")]
    public class DBPageActivity : Activity
    {
        //DBQueryManager m_queryManager = new DBQueryManager();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.db_page_widget);

            // All permissions have to be requested in main activity
            //m_queryManager.Open();
        }

        protected override void OnDestroy()
        {
            //m_queryManager.Close();

            base.OnDestroy();
        }

        [Java.Interop.Export()]
        public void OnRowCountButtonClick(View v)
        {
            //var cnt = m_queryManager.GetGpsLocationCount();
            //FindViewById<TextView>(Resource.Id.row_count_text).Text = cnt.ToString();
        }

        [Java.Interop.Export()]
        public void OnDBBackupButtonClick(View v)
        {
            //var backupPath = m_queryManager.CreateBackup();
            //Toast.MakeText(this, $"Backup location is: {backupPath}", ToastLength.Long).Show();
        }

        [Java.Interop.Export()]
        public void OnDBCleanupButtonClick(View v)
        {
            //ActivityCallbackUtils.ProcessDBCleanupButtonClick(this, m_queryManager);
        }
    }
}