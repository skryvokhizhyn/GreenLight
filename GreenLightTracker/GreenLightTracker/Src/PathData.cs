using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class PathData
    {
        public int Id { get; private set; }
        public List<GpsCoordinate> Points { get; set; } = new List<GpsCoordinate>();

        public PathData()
        {
            Id = PathId.Generate();
        }

        public PathData(int id)
        {
            Id = id;
        }
    }
}