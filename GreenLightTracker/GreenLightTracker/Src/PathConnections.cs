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

        public void Split(int toSplitPathId, int generatedPathId, int causedSplitPathId, bool isInPath)
        {
            if (!m_pathToPathConnections.ContainsKey(toSplitPathId))
                return;

            HashSet<int> currentlyMappedIds;
            m_pathToPathConnections.TryGetValue(toSplitPathId, out currentlyMappedIds);

            m_pathToPathConnections.Remove(toSplitPathId);

            Add(toSplitPathId, generatedPathId);

            if (isInPath)
            {
                Add(causedSplitPathId, generatedPathId);
            }
            else
            {
                Add(toSplitPathId, causedSplitPathId);
            }

            // Move links from original path to newly generated           
            if (currentlyMappedIds != null)
            {
                foreach (var pId in currentlyMappedIds)
                {
                    Add(generatedPathId, pId);
                }
            }

            // Replace links to original path with newly generated one
            foreach (var pathToConnections in m_pathToPathConnections)
            {
                if (pathToConnections.Value.Contains(toSplitPathId))
                {
                    pathToConnections.Value.Remove(toSplitPathId);
                    pathToConnections.Value.Add(generatedPathId);
                }
            }
        }
    }
}