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
        public void DuplicateRoadFilterJointAtStartTest1()
        {
            var filter = new DuplicateRoadFilter(1);
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                    {
                        new GpsCoordinate(){ x = 30 },
                        new GpsCoordinate(){ x = 31 },

                        new GpsCoordinate(){ x = 20 },
                        new GpsCoordinate(){ x = 21 },
                        new GpsCoordinate(){ x = 22 },
                    }, 2);

            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(2, pathList[0].Points.Count);
            Assert.AreEqual(3, pathList[1].Points.Count);
        }

        [Test]
        public void DuplicateRoadFilterJointTest2()
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
            Assert.AreEqual(1, pathList[1].Points.Count);
        }

        [Test]
        public void DuplicateRoadFilterJointTest3()
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
            Assert.AreEqual(2, pathList[1].Points.Count);
            Assert.AreEqual(3, pathList[2].Points.Count);
            Assert.AreEqual(3, pathList[3].Points.Count);
            Assert.AreEqual(1, pathList[4].Points.Count);
        }

        [Test]
        public void DuplicateRoadFilterAtTheEndTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                 new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 1 },
                        new GpsCoordinate(){ x = 2 },

                        new GpsCoordinate(){ x = 10 },
                        new GpsCoordinate(){ x = 11 },
                        new GpsCoordinate(){ x = 12 },
                }, 5);

            var filter = new DuplicateRoadFilter(1);
            filter.Process(pathsData);

            var pathList = (List<PathData>)pathsData;

            Assert.AreEqual(2, pathList.Count);

            Assert.AreEqual(3, pathList[0].Points.Count);
            Assert.AreEqual(3, pathList[1].Points.Count);
        }

        [Test]
        public void DuplicateRoadFilterZigZagTooDistantTest1()
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
            Assert.AreEqual(3, pathList[1].Points.Count);
        }

        [Test]
        public void DuplicateRoadFilterZigZagCloseTest1()
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
        }
    }
}
