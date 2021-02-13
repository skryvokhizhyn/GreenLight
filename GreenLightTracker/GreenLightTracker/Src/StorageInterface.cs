using System.Collections.Generic;
using System;

namespace GreenLightTracker.Src
{
    interface StorageInterface
    {
        void Store(Guid uuid, ICollection<GpsLocation> path);
        long GetGpsLocationCount();
        ICollection<GpsLocation> GetAllGpsLocations();

        void Close();
    }
}