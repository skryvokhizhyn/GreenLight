//using System.Collections.Generic;
//using NUnit.Framework;
//using GreenLightTracker.Src;

//namespace GreenLightTracker.Test
//{
//    [TestFixture]
//    class RoadTrackerTest
//    {
//        [Test]
//        public void GetNeighborsSortedNoPointsTest()
//        {
//            var mapper = new PathMapper(10.0f);
//            var tracker = new RoadTracker(mapper);

//            var nearestNeighborsSorted = tracker.GetNeighbors(false);

//            Assert.IsNull(nearestNeighborsSorted);
//        }

//        [Test]
//        public void GetNeighborsSortedPointsTest()
//        {
//            var mapper = new PathMapper(10);
//            var tracker = new RoadTracker(mapper);

//            var p1 = new GpsCoordinate() { x = 7, y = 5 };

//            mapper.PutPoints(
//                PathPoint.CreateFromPoints(new List<GpsCoordinate>()
//                {
//                    new GpsCoordinate() { x = 5, y = 5 },
//                    p1,
//                    new GpsCoordinate(){ x = 50, y = 5 }
//                }, 0));

//            tracker.TrackPoint(new GpsCoordinate() { x = 10, y = 5 });
//            var nearestNeighborsSorted = tracker.GetNeighbors(true);

//            Assert.AreEqual(1, nearestNeighborsSorted.Count);
//            Assert.AreEqual(p1.x, nearestNeighborsSorted[0].Point.x);
//            Assert.AreEqual(p1.y, nearestNeighborsSorted[0].Point.y);
//        }

//        [Test]
//        public void GetNeighborsEmptyTest()
//        {
//            var mapper = new PathMapper(10);
//            var tracker = new RoadTracker(mapper);

//            tracker.TrackPoint(new GpsCoordinate() { x = 10, y = 5 });
//        }

//        [Test]
//        public void CleanupNeighborsNullTest()
//        {
//            RoadTracker.CleanupNeighbors(null, new GpsCoordinate(), new GpsCoordinate());
//        }

//        [Test]
//        public void CleanupNeighborsEmptyTest()
//        {
//            var n = new List<PathPoint>();
//            RoadTracker.CleanupNeighbors(n, new GpsCoordinate(), new GpsCoordinate());

//            Assert.AreEqual(0, n.Count);
//        }

//        [Test]
//        public void CleanupNeighborsNoNextTest()
//        {
//            var p1 = new GpsCoordinate() { x = 10, y = 0 };
//            var p2 = new GpsCoordinate() { x = 11, y = 0 };

//            var pp1 = new PathPoint() { Point = p1 };
//            var pp2 = new PathPoint() { Point = p2 };

//            var n = new List<PathPoint>() { pp1, pp2 };

//            RoadTracker.CleanupNeighbors(
//                n,
//                new GpsCoordinate() { x = 10, y = 5 },
//                new GpsCoordinate() { x = 11, y = 5 });

//            Assert.AreEqual(0, n.Count);
//        }

//        [Test]
//        public void CleanupNeighborsTooDistantTest()
//        {
//            var p1 = new GpsCoordinate() { x = 10, y = 0 };
//            var p2 = new GpsCoordinate() { x = 11, y = 0 };

//            var pp1 = new PathPoint() { Point = p1 };
//            var pp2 = new PathPoint() { Point = p2 };

//            var n = new List<PathPoint>() { pp1, pp2 };

//            RoadTracker.CleanupNeighbors(
//                n,
//                new GpsCoordinate() { x = 50, y = 5 },
//                new GpsCoordinate() { x = 51, y = 5 });

//            Assert.AreEqual(0, n.Count);
//        }

//        [Test]
//        public void CleanupNeighborsLastNeighborRemovedTest()
//        {
//            var p1 = new GpsCoordinate() { x = 10, y = 0 };
//            var p2 = new GpsCoordinate() { x = 11, y = 0 };

//            var pp2 = new PathPoint() { Point = p2 };
//            var pp1 = new PathPoint() { Point = p1 };

//            PathPoint.AddBerofe(pp2, pp1);

//            var n = new List<PathPoint>() { pp1, pp2 };

//            RoadTracker.CleanupNeighbors(
//                n,
//                new GpsCoordinate() { x = 10, y = 5 },
//                new GpsCoordinate() { x = 11, y = 5 });

//            Assert.AreEqual(1, n.Count);
//        }
//    }
//}
