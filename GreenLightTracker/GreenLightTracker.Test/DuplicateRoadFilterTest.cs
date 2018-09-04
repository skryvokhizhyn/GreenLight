using System.Collections.Generic;
using NUnit.Framework;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    [TestFixture]
    class DuplicateRoadFilterTest
    {
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

            filter.Process(paths);

            var pathList = (List<PathData>)paths;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(3, pathList[1].Points.Count);
        }

        [Test]
        public void DuplicateRoadFilterRemovalAndShorteningTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate(){ x = 30 },
                    new GpsCoordinate(){ x = 31 },

                    new GpsCoordinate(){ x = 29 },
                    new GpsCoordinate(){ x = 30.5 },
                    new GpsCoordinate(){ x = 30.9 },
                    new GpsCoordinate(){ x = 31.5 },
                    new GpsCoordinate(){ x = 32 },
                    new GpsCoordinate(){ x = 33 },
                }, 1.5f);

            var filter = new DuplicateRoadFilter(1);
            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(2, pathList[0].Points.Count);
            Assert.AreEqual(30, pathList[0].Points[0].x);
            Assert.AreEqual(31, pathList[0].Points[1].x);

            Assert.AreEqual(1, pathList[1].Points.Count);
            Assert.AreEqual(33, pathList[1].Points[0].x);
        }

        [Test]
        public void DuplicateRoadFilterRemovalAndSplitTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = 30 },
                        new GpsCoordinate(){ x = 35 },

                        new GpsCoordinate(){ x = 46 },
                        new GpsCoordinate(){ x = 48 },

                        new GpsCoordinate(){ x = 20 },
                        new GpsCoordinate(){ x = 25 },
                        new GpsCoordinate(){ x = 27 },
                        new GpsCoordinate(){ x = 31 },
                        new GpsCoordinate(){ x = 34 },
                        new GpsCoordinate(){ x = 36 },
                        new GpsCoordinate(){ x = 37 },
                        new GpsCoordinate(){ x = 40 },
                        new GpsCoordinate(){ x = 43 },
                        new GpsCoordinate(){ x = 47 },
                        new GpsCoordinate(){ x = 49 },
                        new GpsCoordinate(){ x = 50 },
                }, 5);

            var filter = new DuplicateRoadFilter(1);
            filter.Process(pathsData);

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

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(0, pathList[0].Points[0].x);
            Assert.AreEqual(7, pathList[0].Points[1].x);
            Assert.AreEqual(16, pathList[0].Points[2].x);

            Assert.AreEqual(17, pathList[1].Points[0].x);
        }

        // Commented out due to changed removal of close points logic
        // Now we remove only if roads are colinear
        //[Test]
        //public void DuplicateRoadFilterCrossedTest1()
        //{
        //    var pathsData = PointUtils.CreateFromPoints(
        //        new List<GpsCoordinate>()
        //        {
        //                new GpsCoordinate(){ x = -1 },
        //                new GpsCoordinate(){ x = 0 },
        //                new GpsCoordinate(){ x = 1 },

        //                new GpsCoordinate(){ y = -1.1 },
        //                new GpsCoordinate(){ y = 0 },
        //                new GpsCoordinate(){ y = 1.1 },
        //        }, 1.1f);

        //    var filter = new DuplicateRoadFilter(0.5f);
        //    filter.Process(pathsData);

        //    var pathList = (List<PathData>)pathsData;

        //    Assert.AreEqual(3, pathList.Count);

        //    Assert.AreEqual(3, pathList[0].Points.Count);
        //    Assert.AreEqual(-1, pathList[0].Points[0].x);
        //    Assert.AreEqual(0, pathList[0].Points[1].x);
        //    Assert.AreEqual(1, pathList[0].Points[2].x);

        //    Assert.AreEqual(1, pathList[1].Points.Count);
        //    Assert.AreEqual(-1.1, pathList[1].Points[0].y);

        //    Assert.AreEqual(1, pathList[2].Points.Count);
        //    Assert.AreEqual(1.1, pathList[2].Points[0].y);
        //}
        //[Test]
        //public void DuplicateRoadFilterCrossedTest2()
        //{
        //    var pathsData = PointUtils.CreateFromPoints(
        //        new List<GpsCoordinate>()
        //        {
        //                new GpsCoordinate(){ x = -1 },
        //                new GpsCoordinate(){ x = 0 },
        //                new GpsCoordinate(){ x = 1 },

        //                new GpsCoordinate(){ x = -1, y = -1 },
        //                new GpsCoordinate(){ x = 0.1, y = 0.1 },
        //                new GpsCoordinate(){ x = 1.5, y = 1.5 },
        //        }, 2);

        //    var filter = new DuplicateRoadFilter(0.5f);
        //    filter.Process(pathsData);

        //    var pathList = (List<PathData>)pathsData;

        //    Assert.AreEqual(3, pathList.Count);

        //    Assert.AreEqual(3, pathList[0].Points.Count);
        //    Assert.AreEqual(-1, pathList[0].Points[0].x);
        //    Assert.AreEqual(0, pathList[0].Points[1].x);
        //    Assert.AreEqual(1, pathList[0].Points[2].x);

        //    Assert.AreEqual(1, pathList[1].Points.Count);
        //    Assert.AreEqual(-1, pathList[1].Points[0].y);

        //    Assert.AreEqual(1, pathList[2].Points.Count);
        //    Assert.AreEqual(1.5, pathList[2].Points[0].y);
        //}

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
                        new GpsCoordinate(){ x = 1, y = -0.5 },
                        new GpsCoordinate(){ x = 2, y = -0.6 },
                        new GpsCoordinate(){ x = 3, y = -0.7 },
                }, 2);

            var filter = new DuplicateRoadFilter(3);
            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(3, pathList.Count);

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

            Assert.AreEqual(1, pathList[2].Points.Count);
            Assert.AreEqual(3, pathList[2].Points[0].x);
        }
    }
}
