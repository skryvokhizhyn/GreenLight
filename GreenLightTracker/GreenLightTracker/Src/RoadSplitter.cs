//using System.Collections.Generic;

//namespace GreenLightTracker.Src
//{
//    public class RoadSplitter
//    {
//        private readonly List<PathPoint> m_pathBeginList;
//        private PathPoint m_currentPoint;

//        public RoadSplitter(PathPoint pathBegin)
//        {
//            m_currentPoint = pathBegin;
//            m_pathBeginList = new List<PathPoint>();
//        }

//        public PathPoint Next()
//        {
//            var ret = m_currentPoint;

//            if (m_currentPoint == null)
//            {
//                return null;
//            }

//            m_pathBeginList.Add(m_currentPoint);
//            var currentRoadId = m_currentPoint.PathId;

//            while (m_currentPoint != null &&  currentRoadId == m_currentPoint.PathId)
//            {
//                m_currentPoint = m_currentPoint.Next;
//            }

//            PathPoint.TrimBefore(m_currentPoint);

//            return ret;
//        }
//    }
//}