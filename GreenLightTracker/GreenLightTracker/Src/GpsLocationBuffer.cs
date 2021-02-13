using System.Collections.Generic;
using System;

namespace GreenLightTracker.Src
{
    class GpsLocationBuffer
    {
        private const int BUFFER_SIZE = 300;

        private LinkedList<GpsLocation> m_locations = new LinkedList<GpsLocation>();

        public StorageInterface Storage { private get; set; }
        public Guid? Session { private get; set; }

        public void OnGpsLocationReceived(GpsLocation l)
        {
            l.Timestamp = (int)Java.Lang.JavaSystem.CurrentTimeMillis();

            m_locations.AddLast(l);

            if (m_locations.Count >= BUFFER_SIZE)
                Flush();
        }

        public void Flush()
        {
            if (m_locations.Count > 0)
            {
                Storage.Store(Session.Value, m_locations);
                m_locations.Clear();
            }
        }

        public void Clear()
        {
            m_locations.Clear();
        }
    }
}