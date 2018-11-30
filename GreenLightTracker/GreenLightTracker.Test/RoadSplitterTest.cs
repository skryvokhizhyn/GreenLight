using System.Collections.Generic;
using NUnit.Framework;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    [TestFixture]
    class RoadSplitterTest
    {
        [SetUp]
        public void Setup()
        {
            PathId.Reset();
        }

        [Test]
        public void GpsGpsCoordinateEqalityComparerTest1()
        {
            var comparer = new GpsCoordinateEqalityComparer();

            Assert.IsTrue(comparer.Equals(
                new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 },
                new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 }));

            Assert.IsTrue(comparer.Equals(
               new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 },
               new GpsCoordinate { x = 0.1, y = 0.1, z = 0.100001 }));

            Assert.IsFalse(comparer.Equals(
                new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 },
                new GpsCoordinate { x = -0.1, y = 0.1, z = 0.1 }));

            Assert.IsFalse(comparer.Equals(
                new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 },
                new GpsCoordinate { x = 0.1, y = -0.1, z = 0.1 }));

            Assert.IsFalse(comparer.Equals(
                new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 },
                new GpsCoordinate { x = 0.1, y = 0.1, z = -0.1 }));
        }

        [Test]
        public void GpsGpsCoordinateEqalityComparerTest2()
        {
            var comparer = new GpsCoordinateEqalityComparer();

            Assert.AreEqual(
                comparer.GetHashCode(new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 }),
                comparer.GetHashCode(new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 }));

            Assert.AreEqual(
                comparer.GetHashCode(new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 }),
                comparer.GetHashCode(new GpsCoordinate { x = 0.1, y = 0.1, z = 0.10001 }));

            Assert.AreNotEqual(
                comparer.GetHashCode(new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 }),
                comparer.GetHashCode(new GpsCoordinate { x = -0.1, y = 0.1, z = 0.1 }));

            Assert.AreNotEqual(
                comparer.GetHashCode(new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 }),
                comparer.GetHashCode(new GpsCoordinate { x = 0.1, y = -0.1, z = 0.1 }));

            Assert.AreNotEqual(
                comparer.GetHashCode(new GpsCoordinate { x = 0.1, y = 0.1, z = 0.1 }),
                comparer.GetHashCode(new GpsCoordinate { x = 0.1, y = 0.1, z = -0.1 }));
        }

        [Test]
        public void RoadSplitterOnePathTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                }, 2);

            var splitter = new RoadSplitter(2, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(1, pathList.Count);
            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(1, pathList[0].Points[1].x);
            Assert.AreEqual(2, pathList[0].Points[2].x);
        }

        [Test]
        public void RoadSplitterCrossTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = -1 },
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },

                    new GpsCoordinate(){ y = -1 },
                    new GpsCoordinate(){ y = 0 },
                    new GpsCoordinate(){ y = 1 },
                }, 1);

            var splitter = new RoadSplitter(0.5f, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(-1, pathList[0].Points[0].x);
            Assert.AreEqual(0, pathList[0].Points[1].x);
            Assert.AreEqual(1, pathList[0].Points[2].x);

            Assert.AreEqual(3, pathList[1].Points.Count);
            Assert.AreEqual(-1, pathList[1].Points[0].y);
            Assert.AreEqual(0, pathList[1].Points[1].y);
            Assert.AreEqual(1, pathList[1].Points[2].y);
        }

        [Test]
        public void RoadSplitterTooFarTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = -1 },
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },

                    new GpsCoordinate(){ y = -1 },
                    new GpsCoordinate(){ y = 0.2 },
                    new GpsCoordinate(){ y = 1 },
                }, 1.2f);

            var splitter = new RoadSplitter(0.1f, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(-1, pathList[0].Points[0].x);
            Assert.AreEqual(0, pathList[0].Points[1].x);
            Assert.AreEqual(1, pathList[0].Points[2].x);

            Assert.AreEqual(3, pathList[1].Points.Count);
            Assert.AreEqual(-1, pathList[1].Points[0].y);
            Assert.AreEqual(0.2, pathList[1].Points[1].y);
            Assert.AreEqual(1, pathList[1].Points[2].y);
        }

        [Test]
        public void RoadSplitterFirstShorterEndTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                    new GpsCoordinate(){ x = 3 },

                    new GpsCoordinate(){ x = 0, y = 2 },
                    new GpsCoordinate(){ x = 1, y = 1 },
                    new GpsCoordinate(){ x = 2, y = 0.5 },
                    new GpsCoordinate(){ x = 3, y = 0.1 },
                    new GpsCoordinate(){ x = 4, y = 0.5 },
                }, 2);

            var splitter = new RoadSplitter(1, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(3, pathList.Count);

            Assert.AreEqual(4, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(1, pathList[0].Points[1].x);
            Assert.AreEqual(2, pathList[0].Points[2].x);
            Assert.AreEqual(3, pathList[0].Points[3].x);

            Assert.AreEqual(4, pathList[1].Points.Count);
            Assert.AreEqual(2, pathList[1].Points[0].y);
            Assert.AreEqual(1, pathList[1].Points[1].y);
            Assert.AreEqual(0.5, pathList[1].Points[2].y);
            Assert.AreEqual(0.1, pathList[1].Points[3].y);

            Assert.AreEqual(1, pathList[2].Points.Count);
            Assert.AreEqual(0.5, pathList[2].Points[0].y);
        }

        [Test]
        public void RoadSplitterSecondShorterEndTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                    new GpsCoordinate(){ x = 3 },
                    new GpsCoordinate(){ x = 4 },

                    new GpsCoordinate(){ x = 0, y = 2 },
                    new GpsCoordinate(){ x = 1, y = 1 },
                    new GpsCoordinate(){ x = 2, y = 0.5 },
                    new GpsCoordinate(){ x = 3, y = 0.1 },
                }, 2);

            var splitter = new RoadSplitter(1, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(3, pathList.Count);

            Assert.AreEqual(4, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(1, pathList[0].Points[1].x);
            Assert.AreEqual(2, pathList[0].Points[2].x);
            Assert.AreEqual(3, pathList[0].Points[3].x);

            Assert.AreEqual(4, pathList[1].Points.Count);
            Assert.AreEqual(2, pathList[1].Points[0].y);
            Assert.AreEqual(1, pathList[1].Points[1].y);
            Assert.AreEqual(0.5, pathList[1].Points[2].y);
            Assert.AreEqual(0.1, pathList[1].Points[3].y);

            Assert.AreEqual(1, pathList[2].Points.Count);
            Assert.AreEqual(4, pathList[2].Points[0].x);
        }

        [Test]
        public void RoadSplitterFirstShorterStartTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                    new GpsCoordinate(){ x = 3 },

                    new GpsCoordinate(){ x = -1, y = 0.5 },
                    new GpsCoordinate(){ x = 0, y = 0.1 },
                    new GpsCoordinate(){ x = 1, y = 0.5 },
                    new GpsCoordinate(){ x = 2, y = 1 },
                    new GpsCoordinate(){ x = 3, y = 2 },
                }, 2);

            var splitter = new RoadSplitter(1, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(3, pathList.Count);

            Assert.AreEqual(4, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(1, pathList[0].Points[1].x);
            Assert.AreEqual(2, pathList[0].Points[2].x);
            Assert.AreEqual(3, pathList[0].Points[3].x);

            Assert.AreEqual(2, pathList[1].Points.Count);
            Assert.AreEqual(0.5, pathList[1].Points[0].y);
            Assert.AreEqual(0.1, pathList[1].Points[1].y);

            Assert.AreEqual(3, pathList[2].Points.Count);
            Assert.AreEqual(0.5, pathList[2].Points[0].y);
            Assert.AreEqual(1, pathList[2].Points[1].y);
            Assert.AreEqual(2, pathList[2].Points[2].y);
        }

        [Test]
        public void RoadSplitterSecondShorterStartTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = -1 },
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                    new GpsCoordinate(){ x = 3 },

                    new GpsCoordinate(){ x = 0, y = 0.1 },
                    new GpsCoordinate(){ x = 1, y = 0.5 },
                    new GpsCoordinate(){ x = 2, y = 1 },
                    new GpsCoordinate(){ x = 3, y = 2 },
                }, 2);

            var splitter = new RoadSplitter(1, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(3, pathList.Count);

            Assert.AreEqual(2, pathList[0].Points.Count);
            Assert.AreEqual(-1, pathList[0].Points[0].x);
            Assert.AreEqual(0, pathList[0].Points[1].x);

            Assert.AreEqual(4, pathList[1].Points.Count);
            Assert.AreEqual(0.1, pathList[1].Points[0].y);
            Assert.AreEqual(0.5, pathList[1].Points[1].y);
            Assert.AreEqual(1, pathList[1].Points[2].y);
            Assert.AreEqual(2, pathList[1].Points[3].y);

            Assert.AreEqual(3, pathList[2].Points.Count);
            Assert.AreEqual(1, pathList[2].Points[0].x);
            Assert.AreEqual(2, pathList[2].Points[1].x);
            Assert.AreEqual(3, pathList[2].Points[2].x);
        }

        [Test]
        public void RoadSplitterNoSplitAtEndTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                    new GpsCoordinate(){ x = 3 },

                    new GpsCoordinate(){ x = 0, y = 2 },
                    new GpsCoordinate(){ x = 1, y = 1 },
                    new GpsCoordinate(){ x = 2, y = 0.5 },
                    new GpsCoordinate(){ x = 3, y = 0.1 },
                }, 2);

            var splitter = new RoadSplitter(1, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(4, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(1, pathList[0].Points[1].x);
            Assert.AreEqual(2, pathList[0].Points[2].x);
            Assert.AreEqual(3, pathList[0].Points[3].x);

            Assert.AreEqual(4, pathList[1].Points.Count);
            Assert.AreEqual(2, pathList[1].Points[0].y);
            Assert.AreEqual(1, pathList[1].Points[1].y);
            Assert.AreEqual(0.5, pathList[1].Points[2].y);
            Assert.AreEqual(0.1, pathList[1].Points[3].y);
        }

        [Test]
        public void RoadSplitterNoSplitAtStartTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                    new GpsCoordinate(){ x = 3 },

                    new GpsCoordinate(){ x = 0, y = 0.1 },
                    new GpsCoordinate(){ x = 1, y = 0.5 },
                    new GpsCoordinate(){ x = 2, y = 1 },
                    new GpsCoordinate(){ x = 3, y = 2 },
                }, 2);

            var splitter = new RoadSplitter(1, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(4, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(1, pathList[0].Points[1].x);
            Assert.AreEqual(2, pathList[0].Points[2].x);
            Assert.AreEqual(3, pathList[0].Points[3].x);

            Assert.AreEqual(4, pathList[1].Points.Count);
            Assert.AreEqual(0.1, pathList[1].Points[0].y);
            Assert.AreEqual(0.5, pathList[1].Points[1].y);
            Assert.AreEqual(1, pathList[1].Points[2].y);
            Assert.AreEqual(2, pathList[1].Points[3].y);
        }

        [Test]
        public void RoadSplitterZigZagTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                    new GpsCoordinate(){ x = 3 },
                    new GpsCoordinate(){ x = 4 },
                }, 1);

            paths.AddRange(PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0, y = 0.5 },
                    new GpsCoordinate(){ x = 1, y = 2 },
                    new GpsCoordinate(){ x = 2, y = -0.5 },
                    new GpsCoordinate(){ x = 3, y = -2 },
                    new GpsCoordinate(){ x = 4, y = 0.5 },
                }, 3));

            var splitter = new RoadSplitter(1, null);
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(5, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(1, pathList[0].Points[1].x);
            Assert.AreEqual(2, pathList[0].Points[2].x);
            Assert.AreEqual(3, pathList[0].Points[3].x);
            Assert.AreEqual(4, pathList[0].Points[4].x);

            Assert.AreEqual(5, pathList[1].Points.Count);
            Assert.AreEqual(0.5, pathList[1].Points[0].y);
            Assert.AreEqual(2, pathList[1].Points[1].y);
            Assert.AreEqual(-0.5, pathList[1].Points[2].y);
            Assert.AreEqual(-2, pathList[1].Points[3].y);
            Assert.AreEqual(0.5, pathList[1].Points[4].y);
        }

        [Test]
        public void RoadSplitterCrossNoConnectionTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = -1 },
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },

                    new GpsCoordinate(){ y = -1 },
                    new GpsCoordinate(){ y = 0 },
                    new GpsCoordinate(){ y = 1 },
                }, 1);

            var splitter = new RoadSplitter(0.5f, new PathConnections());
            splitter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(-1, pathList[0].Points[0].x);
            Assert.AreEqual(0, pathList[0].Points[1].x);
            Assert.AreEqual(1, pathList[0].Points[2].x);

            Assert.AreEqual(3, pathList[1].Points.Count);
            Assert.AreEqual(-1, pathList[1].Points[0].y);
            Assert.AreEqual(0, pathList[1].Points[1].y);
            Assert.AreEqual(1, pathList[1].Points[2].y);
        }

        [Test]
        public void RoadSplitterComplexConnectionTest1()
        {
            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 }, // 0
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                    new GpsCoordinate(){ x = 3 },
                    new GpsCoordinate(){ x = 4 },
                    new GpsCoordinate(){ x = 5 },
                    new GpsCoordinate(){ x = 6 },

                    new GpsCoordinate(){ x = 1, y = 2 }, // 1
                    new GpsCoordinate(){ x = 1, y = 1 },
                    new GpsCoordinate(){ x = 1, y = 0.1 },

                    new GpsCoordinate(){ x = 3, y = -0.1 }, // 2
                    new GpsCoordinate(){ x = 3, y = -1 },
                    new GpsCoordinate(){ x = 3, y = -2 },

                    new GpsCoordinate(){ x = 5, y = 2 }, // 3
                    new GpsCoordinate(){ x = 5, y = 1 },
                    new GpsCoordinate(){ x = 5, y = 0.1 },

                    new GpsCoordinate(){ x = 6.1 }, //4
                    new GpsCoordinate(){ x = 7 },

                    new GpsCoordinate(){ x = -2 }, // 5
                    new GpsCoordinate(){ x = -1 },
                    new GpsCoordinate(){ x = -0.1 },
                }, 1);

            var pathConnections = new PathConnections();
            pathConnections.Add(0, 4);
            pathConnections.Add(1, 0);
            pathConnections.Add(0, 2);
            pathConnections.Add(3, 0);
            pathConnections.Add(5, 0);

            var splitter = new RoadSplitter(0.5f, pathConnections);
            splitter.Process(paths);

            var connectionsTracker = new PathConnectionsChecksTracker(pathConnections);

            Assert.IsTrue(connectionsTracker.HasConnection(5, 0));
            Assert.IsTrue(connectionsTracker.HasConnection(0, 6));
            Assert.IsTrue(connectionsTracker.HasConnection(1, 6));
            Assert.IsTrue(connectionsTracker.HasConnection(6, 7));
            Assert.IsTrue(connectionsTracker.HasConnection(6, 2));
            Assert.IsTrue(connectionsTracker.HasConnection(3, 8));
            Assert.IsTrue(connectionsTracker.HasConnection(7, 8));
            Assert.IsTrue(connectionsTracker.HasConnection(8, 4));
            Assert.IsTrue(connectionsTracker.AllConnectionsChecked());
        }
    }
}
