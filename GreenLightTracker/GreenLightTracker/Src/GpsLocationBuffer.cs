﻿using System.Collections.Generic;
using System;

namespace GreenLightTracker.Src
{
    class GpsLocationBuffer
    {
        private const int BUFFER_SIZE = 50;

        private LinkedList<GpsLocation> m_locations = new LinkedList<GpsLocation>();

        public StorageInterface Storage { private get; set; }
        public Guid? Session { private get; set; }

        public void OnGpsLocationReceived(GpsLocation l)
        {
            l.Timestamp = Java.Lang.JavaSystem.CurrentTimeMillis();

            m_locations.AddLast(l);

            if (m_locations.Count >= BUFFER_SIZE)
                Flush();
        }

        public void Flush()
        {
            if (m_locations.Count > 0)
            {
                try
                {
                    Storage.Store(Session.Value, m_locations);
                }
                catch (Exception)
                {
                }

                m_locations.Clear();
            }
        }

        public void Clear() => m_locations.Clear();
    }
}