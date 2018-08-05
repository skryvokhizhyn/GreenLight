using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class PathMapper
    {
        private readonly float m_tolerance;
        private const int GRID_STEP = 100;

        private Dictionary<MapperCell, List<PathPoint>> m_mapper 
            = new Dictionary<MapperCell, List<PathPoint>>();

        private List<PathPoint> m_firstPoints
            = new List<PathPoint>();


        public PathMapper(float tolerance)
        {
            m_tolerance = tolerance;
        }

        public float GetTolerance()
        {
            return m_tolerance;
        }

        public void PutPointList(ICollection<PathData> pathsData)
        {
            foreach (var pathData in pathsData)
            {
                PutPoints(PathPoint.CreateFromPathData(pathData));
            }
        }

        public void PutPoints(PathPoint pathBegin)
        {
            if (pathBegin == null)
                return;

            m_firstPoints.Add(pathBegin);

            PathPoint point = pathBegin;

            while (point != null)
            {
                PutPoint(point);
                point = point.Next;
            }
        }

        private void PutPoint(PathPoint point)
        {
            var cells = GetTouchedCells(point.Point, m_tolerance);

            foreach(var c in cells)
            {
                List<PathPoint> neighbors;

                if (m_mapper.TryGetValue(c, out neighbors))
                {
                    neighbors.Add(point);
                }
                else
                {
                    m_mapper.Add(c, new List<PathPoint> { point });
                }
            }
        }

        public IList<PathPoint> GetNearestPoints(GpsCoordinate point)
        {
            var cell = GetCellByPoint(point);

            List<PathPoint> neighbors;

            m_mapper.TryGetValue(cell, out neighbors);

            return neighbors;
        }

        class PathPointAndDistance
        {
            public PathPoint pathPoint;
            public float distance;
        };

        public List<PathPoint> GetNearestPointsFiltered(GpsCoordinate point)
        {
            var neighbors = GetNearestPoints(point);

            if (neighbors == null)
                return null;

            var closeNeighbors = new List<PathPoint>();

            var routeToData = new Dictionary<int, PathPointAndDistance>();

            foreach (var p in neighbors)
            {
                var dist = PointUtils.GetDistance(p.Point, point);

                if (dist <= m_tolerance)
                {
                    PathPointAndDistance pathData;

                    if (routeToData.TryGetValue(p.PathId, out pathData))
                    {
                        if (dist < pathData.distance)
                        {
                            pathData.pathPoint = p;
                            pathData.distance = dist;
                        }
                    }
                    else
                    {
                        routeToData.Add(p.PathId, new PathPointAndDistance() { pathPoint = p, distance = dist });
                    }
                }
            }

            if (routeToData.Count == 0)
            {
                return null;
            }

            foreach(var p in routeToData)
            {
                closeNeighbors.Add(p.Value.pathPoint);
            }

            return closeNeighbors;
        }

        public static MapperCell GetCellByPoint(GpsCoordinate point)
        {
            int x = (int)point.x;
            int y = (int)point.y;

            var cell = new MapperCell();

            if (x >= 0)
            {
                cell.x = x - x % GRID_STEP;
            }
            else
            {
                cell.x = x - GRID_STEP - x % GRID_STEP;
            }

            if (y >= 0)
            {
                cell.y = y - y % GRID_STEP;
            }
            else
            {
                cell.y = y - GRID_STEP - y % GRID_STEP;
            }

            return cell;
        }

        public static IList<MapperCell> GetTouchedCells(GpsCoordinate point, float tolerance)
        {
            MapperCell cell = GetCellByPoint(point);
            var res = new List<MapperCell> { cell };

            if (point.x - cell.x < tolerance)
            {
                res.Add(new MapperCell() { x = cell.x - GRID_STEP, y = cell.y });
            }

            if (point.y - cell.y < tolerance)
            {
                res.Add(new MapperCell() { x = cell.x, y = cell.y - GRID_STEP });
            }

            MapperCell rightCell = new MapperCell() { x = cell.x + GRID_STEP, y = cell.y };
            MapperCell upCell = new MapperCell() { x = cell.x, y = cell.y + GRID_STEP };

            if (rightCell.x - point.x < tolerance)
            {
                res.Add(rightCell);
            }

            if (upCell.y - point.y < tolerance)
            {
                res.Add(upCell);
            }

            return res;
        }

        public ICollection<GpsCoordinate> GetPoints(int? id = null)
        {
            var res = new List<GpsCoordinate>();

            foreach (var p in m_firstPoints)
            {
                if (id != null && id.Value != p.PathId)
                {
                    continue;
                }

                var point = p;

                while (point != null)
                {
                    res.Add(point.Point);
                    point = point.Next;
                }
            }

            return res;
        }
    }
}