using System.Collections.Generic;
using NUnit.Framework;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    [TestFixture]
    class DuplicateRoadFilterTest
    {
        [SetUp]
        public void Setup()
        {
            PathId.Reset();
        }

        [Test]
        public void DuplicateRoadFilterNothingToRemoveTest1()
        {
            var filter = new DuplicateRoadFilter(10);

            var paths = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },

                    new GpsCoordinate(){ x = 140 },
                    new GpsCoordinate(){ x = 141 },
                    new GpsCoordinate(){ x = 142 },
                }, 2);

            var connections = new PathConnections();
            filter.Process(paths, connections);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(3, pathList[1].Points.Count);

            Assert.IsTrue(connections.IsEmpty());
        }

        [Test]
        public void DuplicateRoadFilter_V_NoParallelPointsTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 2 },
                    new GpsCoordinate(){ x = 3 },
                    new GpsCoordinate(){ x = 4 },
                    new GpsCoordinate(){ x = 5 },


                    new GpsCoordinate(){ x = 0, y = 2 },
                    new GpsCoordinate(){ x = 1, y = 1 },
                    new GpsCoordinate(){ x = 2, y = 0 },
                    new GpsCoordinate(){ x = 3, y = 0 },
                    new GpsCoordinate(){ x = 4, y = 1 },
                    new GpsCoordinate(){ x = 5, y = 2 },
                }, 1.5f);

            var filter = new DuplicateRoadFilter(1);
            var connections = new PathConnections();
            filter.Process(pathsData, connections);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(6, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(1, pathList[0].Points[1].x);
            Assert.AreEqual(2, pathList[0].Points[2].x);
            Assert.AreEqual(3, pathList[0].Points[3].x);
            Assert.AreEqual(4, pathList[0].Points[4].x);
            Assert.AreEqual(5, pathList[0].Points[5].x);

            Assert.AreEqual(6, pathList[1].Points.Count);
            Assert.AreEqual(2, pathList[1].Points[0].y);
            Assert.AreEqual(1, pathList[1].Points[1].y);
            Assert.AreEqual(0, pathList[1].Points[2].y);
            Assert.AreEqual(0, pathList[1].Points[3].y);
            Assert.AreEqual(1, pathList[1].Points[4].y);
            Assert.AreEqual(2, pathList[1].Points[5].y);

            Assert.IsTrue(connections.IsEmpty());
        }

        [Test]
        public void DuplicateRoadFilterSimpleConnectionTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 1 },
                        new GpsCoordinate(){ x = 2 },

                        new GpsCoordinate(){ x = 0, y = 2 },
                        new GpsCoordinate(){ x = 1, y = 1 },
                        new GpsCoordinate(){ x = 2, y = 1 },
                }, 2);

            var filter = new DuplicateRoadFilter(1);
            var connections = new PathConnections();
            filter.Process(pathsData, connections);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.IsFalse(connections.IsEmpty());
            Assert.IsTrue(connections.HasConnection(1, 0));
            Assert.IsTrue(!connections.HasConnection(0, 1));
        }

        [Test]
        public void DuplicateRoadFilterSimpleConnectionTest2()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 1 },
                        new GpsCoordinate(){ x = 2 },

                        new GpsCoordinate(){ x = 0, y = 1 },
                        new GpsCoordinate(){ x = 1, y = 1 },
                        new GpsCoordinate(){ x = 2, y = 2 },
                }, 2);

            var filter = new DuplicateRoadFilter(1);
            var connections = new PathConnections();
            filter.Process(pathsData, connections);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.IsFalse(connections.IsEmpty());
            Assert.IsTrue(connections.HasConnection(0, 1));
            Assert.IsTrue(!connections.HasConnection(1, 0));
        }

        [Test]
        public void DuplicateRoadFilterRemovalAndSplitTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = 30 },//0
                        new GpsCoordinate(){ x = 35 },

                        new GpsCoordinate(){ x = 46 },//1
                        new GpsCoordinate(){ x = 48 },


                        new GpsCoordinate(){ x = 20 },//3
                        new GpsCoordinate(){ x = 25 },
                        new GpsCoordinate(){ x = 27 },

                        new GpsCoordinate(){ x = 31 },//-
                        new GpsCoordinate(){ x = 34 },//-
                        new GpsCoordinate(){ x = 36 },//-

                        new GpsCoordinate(){ x = 37 },//4
                        new GpsCoordinate(){ x = 40 },
                        new GpsCoordinate(){ x = 43 },
                        new GpsCoordinate(){ x = 47 },//-
                        new GpsCoordinate(){ x = 49 },//-
                        new GpsCoordinate(){ x = 50 },//5
                }, 5);

            var filter = new DuplicateRoadFilter(1);
            var connections = new PathConnections();
            filter.Process(pathsData, connections);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(5, pathList.Count);

            Assert.AreEqual(2, pathList[0].Points.Count);
            Assert.AreEqual(30, pathList[0].Points[0].x);
            Assert.AreEqual(35, pathList[0].Points[1].x);

            Assert.AreEqual(2, pathList[1].Points.Count);
            Assert.AreEqual(46, pathList[1].Points[0].x);
            Assert.AreEqual(48, pathList[1].Points[1].x);

            Assert.AreEqual(3, pathList[2].Points.Count);
            Assert.AreEqual(20, pathList[2].Points[0].x);
            Assert.AreEqual(25, pathList[2].Points[1].x);
            Assert.AreEqual(27, pathList[2].Points[2].x);

            Assert.AreEqual(3, pathList[3].Points.Count);
            Assert.AreEqual(37, pathList[3].Points[0].x);
            Assert.AreEqual(40, pathList[3].Points[1].x);
            Assert.AreEqual(43, pathList[3].Points[2].x);

            Assert.AreEqual(1, pathList[4].Points.Count);
            Assert.AreEqual(50, pathList[4].Points[0].x);

            Assert.IsFalse(connections.IsEmpty());
            Assert.IsTrue(connections.HasConnection(3, 0));
            Assert.IsTrue(connections.HasConnection(0, 4));
            Assert.IsTrue(connections.HasConnection(4, 1));
            Assert.IsTrue(connections.HasConnection(1, 5));
        }

        [Test]
        public void DuplicateRoadFilterParallelTooDistantTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 7 },
                        new GpsCoordinate(){ x = 16 },

                        new GpsCoordinate(){ x = 1, y = 5 },
                        new GpsCoordinate(){ x = 9, y = 5 },
                        new GpsCoordinate(){ x = 18, y = 5 },
                }, 10);

            var filter = new DuplicateRoadFilter(5);
            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(7, pathList[0].Points[1].x);
            Assert.AreEqual(16, pathList[0].Points[2].x);

            Assert.AreEqual(3, pathList[1].Points.Count);
            Assert.AreEqual(1, pathList[1].Points[0].x);
            Assert.AreEqual(9, pathList[1].Points[1].x);
            Assert.AreEqual(18, pathList[1].Points[2].x);
        }

        [Test]
        public void DuplicateRoadFilterParallelCloseTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 7 },
                        new GpsCoordinate(){ x = 16 },

                        new GpsCoordinate(){ x = 1, y = 0.1 },
                        new GpsCoordinate(){ x = 8, y = 0.1 },
                        new GpsCoordinate(){ x = 17, y = 0.1 },
                }, 10);

            var filter = new DuplicateRoadFilter(2);
            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(1, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(7, pathList[0].Points[1].x);
            Assert.AreEqual(16, pathList[0].Points[2].x);
        }

        [Test]
        public void DuplicateRoadFilterCrossedTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                            new GpsCoordinate(){ x = -1 },
                            new GpsCoordinate(){ x = 0 },
                            new GpsCoordinate(){ x = 1 },

                            new GpsCoordinate(){ y = -1.1 },
                            new GpsCoordinate(){ y = 0 },
                            new GpsCoordinate(){ y = 1.1 },
                }, 1.1f);

            var filter = new DuplicateRoadFilter(0.5f);
            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(-1, pathList[0].Points[0].x);
            Assert.AreEqual(0, pathList[0].Points[1].x);
            Assert.AreEqual(1, pathList[0].Points[2].x);

            Assert.AreEqual(3, pathList[1].Points.Count);
            Assert.AreEqual(-1.1, pathList[1].Points[0].y);
            Assert.AreEqual(0, pathList[1].Points[1].y);
            Assert.AreEqual(1.1, pathList[1].Points[2].y);
        }

        [Test]
        public void DuplicateRoadFilterCrossedTest2()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = -1 },
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 1 },

                        new GpsCoordinate(){ x = -1, y = -1 },
                        new GpsCoordinate(){ x = 0.1, y = 0.1 },
                        new GpsCoordinate(){ x = 1.5, y = 1.5 },
                }, 2);

            var filter = new DuplicateRoadFilter(0.5f);
            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(-1, pathList[0].Points[0].x);
            Assert.AreEqual(0, pathList[0].Points[1].x);
            Assert.AreEqual(1, pathList[0].Points[2].x);

            Assert.AreEqual(3, pathList[1].Points.Count);
            Assert.AreEqual(-1, pathList[1].Points[0].y);
            Assert.AreEqual(0.1, pathList[1].Points[1].y);
            Assert.AreEqual(1.5, pathList[1].Points[2].y);
        }

        [Test]
        public void DuplicateRoadFilterCheckColinearNoNeighborsTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = -1 },
                }, 2);

            var onlyOnePointInPathPoints = new List<PathPoint> { PathPoint.CreateFromPathData(pathsData[0]) };

            var checkedPoints = new List<GpsCoordinate>()
            {
                    new GpsCoordinate(){ x = -1 },
            };

            Assert.IsFalse(DuplicateRoadFilter.AtLeastOneNeighborIsColinear(null, checkedPoints, 0));
            Assert.IsFalse(DuplicateRoadFilter.AtLeastOneNeighborIsColinear(onlyOnePointInPathPoints, checkedPoints, 0));
            Assert.IsFalse(DuplicateRoadFilter.AtLeastOneNeighborIsColinear(onlyOnePointInPathPoints, checkedPoints, 1));
        }

        [Test]
        public void DuplicateRoadFilterCheckColinearNeighborsTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = -1 },
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 1 },
                }, 2);

            var pathPoints = new List<PathPoint> { PathPoint.CreateFromPathData(pathsData[0]) };

            var checkedPoints = new List<GpsCoordinate>()
            {
                    new GpsCoordinate(){ x = -1 },
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = 1 },
            };

            Assert.IsTrue(DuplicateRoadFilter.AtLeastOneNeighborIsColinear(pathPoints, checkedPoints, 0));
            Assert.IsTrue(DuplicateRoadFilter.AtLeastOneNeighborIsColinear(pathPoints, checkedPoints, 1));
            Assert.IsTrue(DuplicateRoadFilter.AtLeastOneNeighborIsColinear(pathPoints, checkedPoints, 2));
        }

        [Test]
        public void DuplicateRoadFilterCheckColinearReversedNeighborsTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = -1 },
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 1 },

                        new GpsCoordinate(){ x = 1, y = 3 },
                        new GpsCoordinate(){ x = 0, y = 3 },
                        new GpsCoordinate(){ x = -1, y = 3 },
                }, 2);

            var pathPoints = new List<PathPoint> { PathPoint.CreateFromPathData(pathsData[0]), PathPoint.CreateFromPathData(pathsData[1]) };

            var checkedPoints = new List<GpsCoordinate>()
            {
                    new GpsCoordinate(){ x = 1 },
                    new GpsCoordinate(){ x = 0 },
                    new GpsCoordinate(){ x = -1 },
            };

            Assert.IsTrue(DuplicateRoadFilter.AtLeastOneNeighborIsColinear(pathPoints, checkedPoints, 0));
        }

        [Test]
        public void DuplicateRoadFilterKeepReversedTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = -1 },
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 1 },

                        new GpsCoordinate(){ x = 1, y = 3 },
                        new GpsCoordinate(){ x = 0, y = 3 },
                        new GpsCoordinate(){ x = -1, y = 3 },
                }, 2);

            var filter = new DuplicateRoadFilter(2);
            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(-1, pathList[0].Points[0].x);
            Assert.AreEqual(0, pathList[0].Points[1].x);
            Assert.AreEqual(1, pathList[0].Points[2].x);

            Assert.AreEqual(3, pathList[1].Points.Count);
            Assert.AreEqual(1, pathList[1].Points[0].x);
            Assert.AreEqual(0, pathList[1].Points[1].x);
            Assert.AreEqual(-1, pathList[1].Points[2].x);
        }

        [Test]
        public void DuplicateRoadFilterComingCloserAndParallelTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = -3 },
                        new GpsCoordinate(){ x = -2 },
                        new GpsCoordinate(){ x = -1 },
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 1 },
                        new GpsCoordinate(){ x = 2 },
                        new GpsCoordinate(){ x = 3 },

                        new GpsCoordinate(){ x = -3, y = 3 },
                        new GpsCoordinate(){ x = -2, y = 2 },
                        new GpsCoordinate(){ x = -1, y = 1 },
                        new GpsCoordinate(){ x = 0, y = 0 },
                        new GpsCoordinate(){ x = 1, y = -0.1 },
                        new GpsCoordinate(){ x = 2, y = -0.2 },
                        new GpsCoordinate(){ x = 3, y = -0.3 },
                }, 2);

            var filter = new DuplicateRoadFilter(3);
            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(7, pathList[0].Points.Count);
            Assert.AreEqual(-3, pathList[0].Points[0].x);
            Assert.AreEqual(-2, pathList[0].Points[1].x);
            Assert.AreEqual(-1, pathList[0].Points[2].x);
            Assert.AreEqual(0, pathList[0].Points[3].x);
            Assert.AreEqual(1, pathList[0].Points[4].x);
            Assert.AreEqual(2, pathList[0].Points[5].x);
            Assert.AreEqual(3, pathList[0].Points[6].x);

            Assert.AreEqual(4, pathList[1].Points.Count);
            Assert.AreEqual(-3, pathList[1].Points[0].x);
            Assert.AreEqual(-2, pathList[1].Points[1].x);
            Assert.AreEqual(-1, pathList[1].Points[2].x);
            Assert.AreEqual(0, pathList[1].Points[3].x);
        }
    }
}
