using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class PathData
    {
        public int Id { get; private set; }
        public List<GpsCoordinate> Points { get; set; } = new List<GpsCoordinate>();

        public PathData(int? id = null)
        {
            Id = (id == null) ? PathId.Generate() : id.Value;
        }
    }
}