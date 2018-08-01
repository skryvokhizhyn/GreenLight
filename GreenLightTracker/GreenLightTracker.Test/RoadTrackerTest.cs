using System.Collections.Generic;
using NUnit.Framework;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    [TestFixture]
    class RoadTrackerTest
    {
        [Test]
        public void GetNeighborsSortedNoPointsTest()
        {
            var mapper = new PathMapper(10.0f);
            var tracker = new RoadTracker(mapper);

            var nearestNeighborsSorted = tracker.GetNeighbors(false);

            Assert.IsNull(nearestNeighborsSorted);
        }

        [Test]
        public void GetNeighborsNullPointTest()
        {
            var tracker = new RoadTracker(null);

            tracker.TrackPoint(null);
            Assert.IsNull(tracker.GetNeighbors(true));
        }

        public void GetNeighborsEmptyMapperTest()
        {
            var mapper = new PathMapper(10.0f);
            var tracker = new RoadTracker(mapper);

            tracker.TrackPoint(new GpsCoordinate() { x = 10, y = 5 });
            tracker.TrackPoint(new GpsCoordinate() { x = 11, y = 5 });

            Assert.IsNull(tracker.GetNeighbors(false));
        }

        [Test]
        public void GetNeighborsSortedPointsTest()
        {
            var mapper = new PathMapper(10);
            var tracker = new RoadTracker(mapper);

            var p1 = new GpsCoordinate() { x = 7, y = 5 };

            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate() { x = 5, y = 5 },
                    p1,
                    new GpsCoordinate(){ x = 50, y = 5 }
                }, 100);

            mapper.PutPoints(PathPoint.CreateFromPathData(pathsData[0]));

            tracker.TrackPoint(new GpsCoordinate() { x = 10, y = 5 });
            tracker.TrackPoint(new GpsCoordinate() { x = 11, y = 5 });
            var nearestNeighborsSorted = tracker.GetNeighbors(true);

            Assert.AreEqual(1, nearestNeighborsSorted.Count);
            Assert.AreEqual(p1.x, nearestNeighborsSorted[0].Point.x);
            Assert.AreEqual(p1.y, nearestNeighborsSorted[0].Point.y);
        }

        [Test]
        public void GetNeighborsEmptyTest()
        {
            var mapper = new PathMapper(10);
            var tracker = new RoadTracker(mapper);

            tracker.TrackPoint(new GpsCoordinate() { x = 10, y = 5 });

            Assert.IsNull(tracker.GetNeighbors(false));
        }

        [Test]
        public void CleanupNeighborsNullTest()
        {
            RoadTracker.CleanupNeighbors(null, new GpsCoordinate(), new GpsCoordinate(), 10);
        }

        [Test]
        public void CleanupNeighborsEmptyTest()
        {
            var n = new List<PathPoint>();
            RoadTracker.CleanupNeighbors(n, new GpsCoordinate(), new GpsCoordinate(), 10);

            Assert.AreEqual(0, n.Count);
        }

        [Test]
        public void CleanupNeighborsNoNextTest()
        {
            var p1 = new GpsCoordinate() { x = 10, y = 0 };
            var p2 = new GpsCoordinate() { x = 11, y = 0 };

            var pp1 = new PathPoint() { Point = p1 };
            var pp2 = new PathPoint() { Point = p2 };

            var n = new List<PathPoint>() { pp1, pp2 };

            RoadTracker.CleanupNeighbors(
                n,
                new GpsCoordinate() { x = 10, y = 5 },
                new GpsCoordinate() { x = 11, y = 5 },
                10);

            Assert.AreEqual(0, n.Count);
        }

        [Test]
        public void CleanupNeighborsTooDistantTest()
        {
            var p1 = new GpsCoordinate() { x = 10, y = 0 };
            var p2 = new GpsCoordinate() { x = 11, y = 0 };

            var pp1 = new PathPoint() { Point = p1 };
            var pp2 = new PathPoint() { Point = p2 };

            var n = new List<PathPoint>() { pp1, pp2 };

            RoadTracker.CleanupNeighbors(
                n,
                new GpsCoordinate() { x = 50, y = 5 },
                new GpsCoordinate() { x = 51, y = 5 },
                10);

            Assert.AreEqual(0, n.Count);
        }

        [Test]
        public void PromoteNeighborsNullTest()
        {
            Assert.DoesNotThrow(() => RoadTracker.PromoteNeighbors(null, null));
        }

        [Test]
        public void PromoteNeighborsEmptyTest()
        {
            var neighbors = new List<PathPoint>();

            RoadTracker.PromoteNeighbors(neighbors, null);

            Assert.NotNull(neighbors);
            Assert.AreEqual(0, neighbors.Count);
        }

        [Test]
        public void PromoteNeighbors1Test()
        {
            var neighbors = new List<PathPoint>
            {
                new PathPoint()
            };

            RoadTracker.PromoteNeighbors(neighbors, null);

            Assert.NotNull(neighbors);
            Assert.AreEqual(1, neighbors.Count);
        }

        [Test]
        public void PromoteNeighbors2Test()
        {
            var neighbors = new List<PathPoint>
            {
                PathPoint.CreateFromPoints(
                    new List<GpsCoordinate>
                    {
                        new GpsCoordinate() { x = 0 },
                        new GpsCoordinate() { x = 1 }
                    }, 0)
            };

            RoadTracker.PromoteNeighbors(neighbors, new GpsCoordinate() { x = 0.5 });

            Assert.NotNull(neighbors);
            Assert.AreEqual(1, neighbors.Count);
            Assert.AreEqual(0, neighbors[0].Point.x);
        }

        [Test]
        public void PromoteNeighbors3Test()
        {
            var neighbors = new List<PathPoint>
            {
                PathPoint.CreateFromPoints(
                    new List<GpsCoordinate>
                    {
                        new GpsCoordinate() { x = 0 },
                        new GpsCoordinate() { x = 1 }, 
                        new GpsCoordinate() { x = 2 },
                    }, 0)
            };

            RoadTracker.PromoteNeighbors(neighbors, new GpsCoordinate() { x = 1, y = 0.5 });

            Assert.NotNull(neighbors);
            Assert.AreEqual(1, neighbors.Count);
            Assert.AreEqual(1, neighbors[0].Point.x);
        }

        [Test]
        public void PromoteNeighbors4Test()
        {
            var neighbors = new List<PathPoint>
            {
                PathPoint.CreateFromPoints(
                    new List<GpsCoordinate>
                    {
                        new GpsCoordinate() { x = 0 },
                        new GpsCoordinate() { x = 1 },
                        new GpsCoordinate() { x = 2 },
                    }, 0)
            };

            RoadTracker.PromoteNeighbors(neighbors, new GpsCoordinate() { x = 1, y = -0.5 });

            Assert.NotNull(neighbors);
            Assert.AreEqual(1, neighbors.Count);
            Assert.AreEqual(1, neighbors[0].Point.x);
        }

        [Test]
        public void PromoteNeighborsEndOfPathTest()
        {
            var neighbors = new List<PathPoint>
            {
                PathPoint.CreateFromPoints(
                    new List<GpsCoordinate>
                    {
                        new GpsCoordinate() { x = 0 },
                        new GpsCoordinate() { x = 1 },
                        new GpsCoordinate() { x = 2 },
                    }, 0)
            };

            RoadTracker.PromoteNeighbors(neighbors, new GpsCoordinate() { x = 2.1 });

            Assert.NotNull(neighbors);
            Assert.AreEqual(0, neighbors.Count);
        }

        [Test]
        public void PromoteNeighbors2Neighbors1Test()
        {
            var neighbors = new List<PathPoint>
            {
                PathPoint.CreateFromPoints(
                    new List<GpsCoordinate>
                    {
                        new GpsCoordinate() { x = 0 },
                        new GpsCoordinate() { x = 1 },
                        new GpsCoordinate() { x = 2 },
                    }, 0),
                new PathPoint()
            };

            RoadTracker.PromoteNeighbors(neighbors, new GpsCoordinate() { x = 1.5 });

            Assert.NotNull(neighbors);
            Assert.AreEqual(1, neighbors.Count);
            Assert.AreEqual(1, neighbors[0].Point.x);
        }

        [Test]
        public void PromoteNeighbors2Neighbors11Test()
        {
            var neighbors = new List<PathPoint>
            {
                new PathPoint(),
                PathPoint.CreateFromPoints(
                    new List<GpsCoordinate>
                    {
                        new GpsCoordinate() { x = 0 },
                        new GpsCoordinate() { x = 1 },
                        new GpsCoordinate() { x = 2 },
                    }, 0),
            };

            RoadTracker.PromoteNeighbors(neighbors, new GpsCoordinate() { x = 1.5 });

            Assert.NotNull(neighbors);
            Assert.AreEqual(1, neighbors.Count);
            Assert.AreEqual(1, neighbors[0].Point.x);
        }

        [Test]
        public void PromoteNeighbors2Neighbors2Test()
        {
            var neighbors = new List<PathPoint>
            {
                PathPoint.CreateFromPoints(
                    new List<GpsCoordinate>
                    {
                        new GpsCoordinate() { x = 0 },
                        new GpsCoordinate() { x = 1 },
                        new GpsCoordinate() { x = 2 },
                    }, 0),
                PathPoint.CreateFromPoints(
                    new List<GpsCoordinate>
                    {
                        new GpsCoordinate() { x = 0 },
                        new GpsCoordinate() { x = 1 },
                        new GpsCoordinate() { x = 2 },
                    }, 1),
            };

            RoadTracker.PromoteNeighbors(neighbors, new GpsCoordinate() { x = 1.5 });

            Assert.NotNull(neighbors);
            Assert.AreEqual(2, neighbors.Count);
            Assert.AreEqual(1, neighbors[0].Point.x);
            Assert.AreEqual(1, neighbors[1].Point.x);
        }

        [Test]
        public void GlobalOnePathBackTest1()
        {
            var pathPoints = new List<PathData>
            {
                new PathData(0)
            };

            pathPoints[0].Points.AddRange(new List<GpsCoordinate>
            {
                new GpsCoordinate{ x = 10 },
                new GpsCoordinate{ x = 20 },
                new GpsCoordinate{ x = 30 },
            });

            var pathMapper = new PathMapper(10);
            pathMapper.PutPointList(pathPoints);

            var roadTracker = new RoadTracker();
            roadTracker.SetMapper(pathMapper);

            roadTracker.TrackPoint(new GpsCoordinate { x = 25 });

            var neighbors1 = (List<PathPoint>)roadTracker.GetNeighbors(false);

            Assert.AreEqual(1, neighbors1.Count);
            Assert.AreEqual(0, neighbors1[0].PathId);
            Assert.AreEqual(20, neighbors1[0].Point.x);

            roadTracker.TrackPoint(new GpsCoordinate { x = 0 });

            var neighbors2 = (List<PathPoint>)roadTracker.GetNeighbors(false);

            Assert.AreEqual(1, neighbors2.Count);
            Assert.AreEqual(0, neighbors2[0].PathId);
            Assert.AreEqual(10, neighbors2[0].Point.x);
        }

        [Test]
        public void GlobalOnePathForwardTest1()
        {
            var pathPoints = new List<PathData>
            {
                new PathData(0)
            };

            pathPoints[0].Points.AddRange(new List<GpsCoordinate>
            {
                new GpsCoordinate{ x = 10 },
                new GpsCoordinate{ x = 20 },
                new GpsCoordinate{ x = 30 },
                new GpsCoordinate{ x = 40 },
            });

            var pathMapper = new PathMapper(10);
            pathMapper.PutPointList(pathPoints);

            var roadTracker = new RoadTracker();
            roadTracker.SetMapper(pathMapper);

            roadTracker.TrackPoint(new GpsCoordinate { x = 15 });

            var neighbors1 = (List<PathPoint>)roadTracker.GetNeighbors(false);

            Assert.AreEqual(1, neighbors1.Count);
            Assert.AreEqual(0, neighbors1[0].PathId);
            Assert.AreEqual(10, neighbors1[0].Point.x);

            roadTracker.TrackPoint(new GpsCoordinate { x = 25 });

            var neighbors2 = (List<PathPoint>)roadTracker.GetNeighbors(false);

            Assert.AreEqual(1, neighbors2.Count);
            Assert.AreEqual(0, neighbors2[0].PathId);
            Assert.AreEqual(30, neighbors2[0].Point.x);
        }

        [Test]
        public void GlobalTwoPathForwardTest1()
        {
            var pathPoints = new List<PathData>
            {
                new PathData(0),
                new PathData(1)
            };

            pathPoints[0].Points.AddRange(new List<GpsCoordinate>
            {
                new GpsCoordinate{ x = 10 },
                new GpsCoordinate{ x = 20 },
                new GpsCoordinate{ x = 30 },
                new GpsCoordinate{ x = 40 },
            });

            pathPoints[0].Points.AddRange(new List<GpsCoordinate>
            {
                new GpsCoordinate{ x = 15 },
                new GpsCoordinate{ x = 25 },
                new GpsCoordinate{ x = 35 },
                new GpsCoordinate{ x = 45 },
            });

            var pathMapper = new PathMapper(10);
            pathMapper.PutPointList(pathPoints);

            var roadTracker = new RoadTracker();
            roadTracker.SetMapper(pathMapper);

            roadTracker.TrackPoint(new GpsCoordinate { x = 15 });

            var neighbors1 = (List<PathPoint>)roadTracker.GetNeighbors(false);

            Assert.AreEqual(2, neighbors1.Count);
            Assert.AreEqual(0, neighbors1[0].PathId);
            Assert.AreEqual(10, neighbors1[0].Point.x);
            Assert.AreEqual(1, neighbors1[1].PathId);
            Assert.AreEqual(15, neighbors1[1].Point.x);

            roadTracker.TrackPoint(new GpsCoordinate { x = 25 });

            var neighbors2 = (List<PathPoint>)roadTracker.GetNeighbors(false);

            Assert.AreEqual(2, neighbors1.Count);
            Assert.AreEqual(0, neighbors2[0].PathId);
            Assert.AreEqual(30, neighbors2[0].Point.x);
            Assert.AreEqual(1, neighbors2[1].PathId);
            Assert.AreEqual(30, neighbors2[1].Point.x);
        }

        [Test]
        public void GlobalTwoPathMixedTest1()
        {
            var pathPoints = new List<PathData>
            {
                new PathData(0),
                new PathData(1)
            };

            pathPoints[0].Points.AddRange(new List<GpsCoordinate>
            {
                new GpsCoordinate{ x = 10 },
                new GpsCoordinate{ x = 20 },
                new GpsCoordinate{ x = 30 },
                new GpsCoordinate{ x = 40 },
            });

            pathPoints[1].Points.AddRange(new List<GpsCoordinate>
            {
                new GpsCoordinate{ x = 45 },
                new GpsCoordinate{ x = 35 },
                new GpsCoordinate{ x = 25 },
                new GpsCoordinate{ x = 15 },
            });

            var pathMapper = new PathMapper(10);
            pathMapper.PutPointList(pathPoints);

            var roadTracker = new RoadTracker();
            roadTracker.SetMapper(pathMapper);

            roadTracker.TrackPoint(new GpsCoordinate { x = 15 });

            var neighbors1 = (List<PathPoint>)roadTracker.GetNeighbors(false);

            Assert.AreEqual(2, neighbors1.Count);
            Assert.AreEqual(1, neighbors1[0].PathId);
            Assert.AreEqual(15, neighbors1[0].Point.x);
            Assert.AreEqual(0, neighbors1[1].PathId);
            Assert.AreEqual(10, neighbors1[1].Point.x);

            roadTracker.TrackPoint(new GpsCoordinate { x = 25 });

            var neighbors2 = (List<PathPoint>)roadTracker.GetNeighbors(false);

            Assert.AreEqual(1, neighbors1.Count);
            Assert.AreEqual(0, neighbors2[0].PathId);
            Assert.AreEqual(30, neighbors2[0].Point.x);
        }
    }
}
