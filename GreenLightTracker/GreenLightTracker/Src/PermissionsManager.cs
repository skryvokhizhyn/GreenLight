using System.Collections.Generic;

using Android.Content.PM;
using Android.OS;
using Android.App;

using NLog;

namespace GreenLightTracker.Src
{
    class PermissionsManager
    {
        const int RequestId = 1324657;

        private static Logger m_logger = LogManager.GetCurrentClassLogger();

        private readonly Activity m_activity;

        public PermissionsManager(Activity activity)
        {
            m_activity = activity;
        }

        public void RequestPermissions()
        {
            var permissions = new List<string>();

            if ((int)Build.VERSION.SdkInt >= (int)BuildVersionCodes.M)
            {
                if (m_activity.CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) != Permission.Granted)
                    permissions.Add(Android.Manifest.Permission.AccessFineLocation);

                if (m_activity.CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                    permissions.Add(Android.Manifest.Permission.ReadExternalStorage);

                if (m_activity.CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                    permissions.Add(Android.Manifest.Permission.WriteExternalStorage);
            }

            if (permissions.Count > 0)
            {
                m_logger.Trace("Requesting permissions: " + permissions.ToString());
                m_activity.RequestPermissions(permissions.ToArray(), RequestId);
            }
            else
            {
                m_logger.Trace("Requesting permissions: All granted");
                m_activity.OnRequestPermissionsResult(RequestId, new string[0], new Permission[0]);
            }
        }

        public IEnumerable<string> ProcessPermissionsResult(
            int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RequestId)
            {
                var failedPermissions = new List<string>();

                for (var i = 0; i < permissions.Length; ++i)
                {
                    if (grantResults[i] != Permission.Granted)
                    {
                        failedPermissions.Add(permissions[i]);
                    }
                }

                if (failedPermissions.Count > 0)
                    return failedPermissions;
            }

            return null;
        }
    }
}