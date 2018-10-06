using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace GreenLightTracker.Src
{
    class WakeLockWrapper
    {
        PowerManager.WakeLock m_wakeLock;

        public WakeLockWrapper(ContextWrapper contextWrapper)
        {
            var powerManager = (PowerManager)contextWrapper.GetSystemService(Context.PowerService);
            m_wakeLock = powerManager.NewWakeLock(WakeLockFlags.ScreenBright, "tracking_wake_lock");
        }

        public void Acquire()
        {
            if (!m_wakeLock.IsHeld)
            {
                m_wakeLock.Acquire();
            }
        }

        public void Release()
        {
            if (m_wakeLock.IsHeld)
            {
                m_wakeLock.Release();
            }
        }
    }
}