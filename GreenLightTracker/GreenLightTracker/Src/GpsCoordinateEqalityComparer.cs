using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    public class GpsCoordinateEqalityComparer : IEqualityComparer<GpsCoordinate>
    {
        private readonly float PRECISION = 1000.0f;

        public int GetHashCode(GpsCoordinate p)
        {
            int x = (int)(p.x * PRECISION);
            int y = (int)(p.y * PRECISION);
            int z = (int)(p.z * PRECISION);

            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            hash = hash * 23 + z.GetHashCode();
            return hash;
        }

        public bool Equals(GpsCoordinate lp, GpsCoordinate rp)
        {
            int lpx = (int)(lp.x * PRECISION);
            int lpy = (int)(lp.y * PRECISION);
            int lpz = (int)(lp.z * PRECISION);

            int rpx = (int)(rp.x * PRECISION);
            int rpy = (int)(rp.y * PRECISION);
            int rpz = (int)(rp.z * PRECISION);

            return lpx == rpx && lpy == rpy && lpz == rpz;
        }
    }
}