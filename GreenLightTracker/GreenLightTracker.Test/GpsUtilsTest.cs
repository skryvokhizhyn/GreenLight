using System.Collections.Generic;
using NUnit.Framework;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    [TestFixture]
    class GpsUtilsTest
    {
        [SetUp]
        public void Setup()
        {
            PathId.Reset();
        }

        [Test]
        public void GetPointsFromLocationsTest1()
        {
            var locations = new List<GpsLocation>()
            {
                new GpsLocation{Timestamp = 0},
                new GpsLocation{Timestamp = 1},
                new GpsLocation{Timestamp = 2},
                new GpsLocation{Timestamp = 10},
                new GpsLocation{Timestamp = 11},
                new GpsLocation{Timestamp = 20}
            };

            var paths = (List<PathData>)GpsUtils.GetPathPointsFromLocations(locations, 1);

            Assert.AreEqual(3, paths.Count);

            Assert.AreEqual(0, paths[0].Id);
            Assert.AreEqual(3, paths[0].Points.Count);

            Assert.AreEqual(1, paths[1].Id);
            Assert.AreEqual(2, paths[1].Points.Count);

            Assert.AreEqual(2, paths[2].Id);
            Assert.AreEqual(1, paths[2].Points.Count);
        }
    }
}
