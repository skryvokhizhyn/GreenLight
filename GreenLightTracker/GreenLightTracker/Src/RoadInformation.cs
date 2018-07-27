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
    class RoadInformation
    {
        private int m_roadLightId = -1;

        public delegate void NewRoadLightHandler(int id);
        public event NewRoadLightHandler RoadLightFound;

        public void OnPathPointFound(PathPoint p)
        {
            int foundRoadLightId = p != null ? p.PathId : -1;

            if (m_roadLightId != foundRoadLightId)
            {
                m_roadLightId = foundRoadLightId;

                RoadLightFound(m_roadLightId);
            }
        }
    }
}