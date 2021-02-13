using System.Collections.Generic;
using System;

namespace GreenLightTracker.Src
{
    interface StorageInterface
    {
        void Store(Guid uuid, ICollection<GpsLocation> path);
        ICollection<GpsLocation> GetAllGpsLocations();

        void Close();
    }
}