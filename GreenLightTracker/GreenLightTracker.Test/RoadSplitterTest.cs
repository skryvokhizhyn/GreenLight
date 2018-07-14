using System.Collections.Generic;
using NUnit.Framework;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    [TestFixture]
    class RoadSplitterTest
    {
        //[Test]
        //public void RoadSplitterTrimTest1()
        //{
        //    var points = PointUtils.CreateFromPoints(new List<GpsCoordinate>()
        //        {
        //            new GpsCoordinate() { x = 10 },
        //            new GpsCoordinate() { x = 11 },

        //            new GpsCoordinate() { x = 20 },
        //            new GpsCoordinate() { x = 21 },
        //        }, 5);

        //    var splitter = new RoadSplitter(points);

        //    var road = splitter.Next();

        //    Assert.IsNotNull(road);
        //    Assert.AreEqual(10, road.Point.x);
        //    Assert.AreEqual(11, road.Next.Point.x);
        //    Assert.IsNull(road.Next.Next);

        //    road = splitter.Next();

        //    Assert.IsNotNull(road);
        //    Assert.IsNull(road.Prev);
        //    Assert.AreEqual(12, road.Point.x);
        //    Assert.AreEqual(13, road.Next.Point.x);
        //    Assert.IsNull(road.Next.Next);

        //    Assert.IsNull(splitter.Next());
        //}

        //[Test]
        //public void RoadSplitterGetSubroadsTest()
        //{
        //    var points = new List<GpsCoordinate>()
        //    {
        //        new GpsCoordinate() { x = 10 },
        //        new GpsCoordinate() { x = 11 },
        //        new GpsCoordinate() { x = 12 },
        //        new GpsCoordinate() { x = 30 },
        //        new GpsCoordinate() { x = 31 },
        //        new GpsCoordinate() { x = 50 },
        //    };

        //    var roads = new RoadSplitter(points, 10);

        //    var r = roads.Next();
        //    Assert.NotNull(r);

        //    Assert.AreEqual(3, r.Count);

        //    r = roads.Next();
        //    Assert.NotNull(r);

        //    Assert.AreEqual(2, r.Count);

        //    r = roads.Next();
        //    Assert.NotNull(r);

        //    Assert.AreEqual(1, r.Count);

        //    Assert.IsNull(roads.Next());
        //}

        //[Test]
        //public void RoadSplitterEmtpyTest()
        //{
        //    var points = new List<GpsCoordinate>();
        //    var roads = new RoadSplitter(points, 10);

        //    Assert.IsNull(roads.Next());
        //}

        //[Test]
        //public void RoadSplitterUseCaseTest1()
        //{
        //    var points = new List<GpsCoordinate>
        //    {
        //        new GpsCoordinate() { x = 5, y = 5, z = 0 },
        //        new GpsCoordinate() { x = 7, y = 5, z = 0 },
        //        new GpsCoordinate() { x = 10, y = 100, z = 0 }
        //    };

        //    var roads = new RoadSplitter(points, 10);

        //    var r = roads.Next();
        //    Assert.NotNull(r);

        //    Assert.AreEqual(2, r.Count);

        //    r = roads.Next();
        //    Assert.NotNull(r);

        //    Assert.AreEqual(1, r.Count);

        //    Assert.IsNull(roads.Next());
        //}
    }
}
