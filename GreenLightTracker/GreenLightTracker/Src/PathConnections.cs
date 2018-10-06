using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class PathConnections
    {
        private Dictionary<int, HashSet<int>> m_pathToPathConnections;

        public PathConnections()
        {
            m_pathToPathConnections = new Dictionary<int, HashSet<int>>();
        }

        public void Add(int pathIdFrom, int pathIdTo)
        {
            HashSet<int> mappedIds;
            if (m_pathToPathConnections.TryGetValue(pathIdFrom, out mappedIds))
            {
                mappedIds.Add(pathIdTo);
            }
            else
            {
                m_pathToPathConnections.Add(pathIdFrom, new HashSet<int> { pathIdTo });
            }
        }

        public bool HasConnection(int pathIdFrom, int pathIdTo)
        {
            HashSet<int> mappedIds;
            if (!m_pathToPathConnections.TryGetValue(pathIdFrom, out mappedIds))
            {
                return false;
            }

            return mappedIds.Contains(pathIdTo);
        }

        public bool IsEmpty()
        {
            return m_pathToPathConnections.Count == 0;
        }
    }
}