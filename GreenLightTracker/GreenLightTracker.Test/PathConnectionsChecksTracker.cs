using System.Collections.Generic;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    class PathConnectionsChecksTracker
    {
        private readonly PathConnections m_connections;
        private readonly Dictionary<int, HashSet<int>> m_checkedConnections;

        public PathConnectionsChecksTracker(PathConnections connections)
        {
            m_connections = connections;
            m_checkedConnections = new Dictionary<int, HashSet<int>>();
        }

        public bool HasConnection(int fromId, int toId)
        {
            RegisterConnectionCheck(fromId, toId);
            return m_connections.HasConnection(fromId, toId);
        }

        public bool AllConnectionsChecked()
        {
            foreach (var keyToValue in m_checkedConnections)
            {
                foreach (var toValue in keyToValue.Value)
                {
                    m_connections.Remove(keyToValue.Key, toValue);
                }
            }

            return m_connections.IsEmpty();
        }

        public bool IsEmpty()
        {
            return m_connections.IsEmpty();
        }

        private void RegisterConnectionCheck(int fromId, int toId)
        {
            HashSet<int> mappedIds;
            if (!m_checkedConnections.TryGetValue(fromId, out mappedIds))
            {
                m_checkedConnections.Add(fromId, new HashSet<int>());
                mappedIds = m_checkedConnections[fromId];
            }

            mappedIds.Add(toId);
        }
    }
}
