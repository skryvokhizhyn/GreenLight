using System;
using System.Collections.Generic;
using NUnit.Framework;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    [TestFixture]
    class PointUtilsTest
    {
        [SetUp]
        public void Setup()
        {
            PathId.Reset();
        }

        [TestCase(10, 10, 20, 10, 10)]
        [TestCase(10, 20, 10, 10, 10)]
        [TestCase(-10, 10, 10, 10, 20)]
        public void GetDistanceTest(float p1x, float p1y, float p2x, float p2y, float expectedDist)
        {
            var dist = PointUtils.GetDistance(
                new GpsCoordinate() { x = p1x, y = p1y, z = 0 },
                new GpsCoordinate() { x = p2x, y = p2y, z = 0 }
            );

            Assert.AreEqual(expectedDist, dist);
        }

        [Test]
        public void CreateFromPointsTest1()
        {
            var points = new List<GpsCoordinate>()
            {
                new GpsCoordinate() { x = 0, y = 0, z = 0 },
                new GpsCoordinate() { x = 1, y = 0, z = 0 },
                new GpsCoordinate() { x = 2, y = 0, z = 0 },
                new GpsCoordinate() { x = 10, y = 0, z = 0 },
                new GpsCoordinate() { x = 11, y = 0, z = 0 },
                new GpsCoordinate() { x = 20, y = 0, z = 0 }
            };

            var paths = (List<PathData>)PointUtils.CreateFromPoints(points, 1.0f);

            Assert.AreEqual(3, paths.Count);
            Assert.AreEqual(0, paths[0].Id);
            Assert.AreEqual(3, paths[0].Points.Count);

            Assert.AreEqual(1, paths[1].Id);
            Assert.AreEqual(2, paths[1].Points.Count);

            Assert.AreEqual(2, paths[2].Id);
            Assert.AreEqual(1, paths[2].Points.Count);
        }

        [Test]
        public void CreateFromPointsTest2()
        {
            var points = new List<GpsCoordinate>()
            {
                new GpsCoordinate(){ x = 30 },
                new GpsCoordinate(){ x = 31 },

                new GpsCoordinate(){ x = 29 },
                new GpsCoordinate(){ x = 30.5 },
                new GpsCoordinate(){ x = 30.9 },
                new GpsCoordinate(){ x = 31.5 },
                new GpsCoordinate(){ x = 32 },
            };

            var paths = (List<PathData>)PointUtils.CreateFromPoints(points, 1.5f);

            Assert.AreEqual(2, paths.Count);
            Assert.AreEqual(0, paths[0].Id);
            Assert.AreEqual(2, paths[0].Points.Count);

            Assert.AreEqual(1, paths[1].Id);
            Assert.AreEqual(5, paths[1].Points.Count);
        }

        /*
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
                }, 2);
         */

        //[Test]
        //public void FilterClosePointsGeneralTest()
        //{
        //    var points = new LinkedList<GpsCoordinate>();
        //    points.AddLast(new GpsCoordinate() { x = 0, y = 0, z = 0 });
        //    points.AddLast(new GpsCoordinate() { x = 5, y = 0, z = 0 });
        //    points.AddLast(new GpsCoordinate() { x = 10, y = 0, z = 0 });
        //    points.AddLast(new GpsCoordinate() { x = 20, y = 0, z = 0 });

        //    var path = PathPoint.CreateFromPoints(points, 0);

        //    PointUtils.FilterClosePoints(path, 10);

        //    Assert.AreEqual(3, PointUtils.GetLength(path));
        //}

        //[Test]
        //public void FilterClosePointsAllNearTest()
        //{
        //    var points = new LinkedList<GpsCoordinate>();
        //    points.AddLast(new GpsCoordinate() { x = 0, y = 0, z = 0 });
        //    points.AddLast(new GpsCoordinate() { x = 5, y = 0, z = 0 });
        //    points.AddLast(new GpsCoordinate() { x = 10, y = 0, z = 0 });
        //    points.AddLast(new GpsCoordinate() { x = 20, y = 0, z = 0 });

        //    var path = PathPoint.CreateFromPoints(points, 0);

        //    PointUtils.FilterClosePoints(path, 100);

        //    Assert.IsNotNull(path);
        //    path = path.Next;
        //    Assert.IsNotNull(path);
        //    Assert.IsNull(path.Next);
        //}

        [TestCase(10, 10, 100, 100, 10, true)]
        [TestCase(1, 0, 0, 1, 90, true)]
        [TestCase(10, 1, 10, 0, 10, true)]
        [TestCase(10, 0, 10, 1, 0, false)]
        [TestCase(10, 0, -1, 10, 180, true)]
        [TestCase(10, 0, -1, 10, 90, false)]
        public void CheckColinearTest1(float x1, float y1, float x2, float y2, float tolerance, bool isColinear)
        {
            Assert.AreEqual(isColinear,
                PointUtils.CheckColinear(
                    new GpsCoordinate() { x = x1, y = y1 },
                    new GpsCoordinate() { x = x2, y = y2 },
                    tolerance));
        }

        [Test]
        public void EnrichWithIntemediatePointsEmptyTest1()
        {
            List<PathData> pathList = null;

            PointUtils.EnrichWithIntemediatePoints(pathList, 1.0f);

            Assert.IsNull(pathList);
        }

        [Test]
        public void EnrichWithIntemediatePointsSingleTest1()
        {
            var paths = PointUtils.CreateFromPoints(new List<GpsCoordinate>()
            {
                new GpsCoordinate() { x = 5, y = 5 }
            }, 10);

            PointUtils.EnrichWithIntemediatePoints(paths, 1.0f);

            Assert.IsNotNull(paths);
            Assert.AreEqual(1, paths.Count);
            Assert.AreEqual(1, paths[0].Points.Count);
        }

        [Test]
        public void EnrichWithIntemediatePointsNoEnrichmentTest1()
        {
            var paths = PointUtils.CreateFromPoints(new List<GpsCoordinate>()
            {
                new GpsCoordinate() { x = 5, y = 5 },
                new GpsCoordinate() { x = 5.5, y = 5 }
            }, 10);

            PointUtils.EnrichWithIntemediatePoints(paths, 1.0f);

            Assert.IsNotNull(paths);
            Assert.AreEqual(1, paths.Count);
            Assert.AreEqual(2, paths[0].Points.Count);
        }

        [Test]
        public void EnrichWithIntemediatePointsEnrichmentTest1()
        {
            var paths = PointUtils.CreateFromPoints(new List<GpsCoordinate>()
            {
                new GpsCoordinate() { x = 5, y = 5 },
                new GpsCoordinate() { x = 7, y = 5 }
            }, 10);

            PointUtils.EnrichWithIntemediatePoints(paths, 1.0f);

            Assert.IsNotNull(paths);
            Assert.AreEqual(1, paths.Count);
            Assert.AreEqual(3, paths[0].Points.Count);

            Assert.AreEqual(5.0f, paths[0].Points[0].x);
            Assert.AreEqual(6.0f, paths[0].Points[1].x);
            Assert.AreEqual(7.0f, paths[0].Points[2].x);
        }

        [Test]
        public void EnrichWithIntemediatePointsEnrichmentTest2()
        {
            var paths = PointUtils.CreateFromPoints(new List<GpsCoordinate>()
            {
                new GpsCoordinate() { x = 5, y = 5 },
                new GpsCoordinate() { x = 7, y = 5 },

                new GpsCoordinate() { x = 10, y = 5 },
                new GpsCoordinate() { x = 12, y = 5 }
            }, 2);

            PointUtils.EnrichWithIntemediatePoints(paths, 1.0f);

            Assert.IsNotNull(paths);
            Assert.AreEqual(2, paths.Count);

            Assert.AreEqual(3, paths[0].Points.Count);

            Assert.AreEqual(5.0f, paths[0].Points[0].x);
            Assert.AreEqual(6.0f, paths[0].Points[1].x);
            Assert.AreEqual(7.0f, paths[0].Points[2].x);

            Assert.AreEqual(3, paths[1].Points.Count);

            Assert.AreEqual(10.0f, paths[1].Points[0].x);
            Assert.AreEqual(11.0f, paths[1].Points[1].x);
            Assert.AreEqual(12.0f, paths[1].Points[2].x);
        }

        //[Test]
        //public void GetPointOnPathEmptyTest1()
        //{
        //    Assert.Throws<ArgumentOutOfRangeException>(() => PointUtils.GetPointOnPath(null, 1));
        //}

        //[Test]
        //public void GetPointOnPathOutOfRangeTest1()
        //{
        //    PathPoint pathPoints = PathPoint.CreateFromPoints(new List<GpsCoordinate>()
        //    {
        //        new GpsCoordinate() { x = 5, y = 5 },
        //        new GpsCoordinate() { x = 7, y = 5 }
        //    }, 0);

        //    Assert.Throws<ArgumentOutOfRangeException>(() => PointUtils.GetPointOnPath(pathPoints, 2));
        //}

        //[TestCase(0, 5)]
        //[TestCase(1, 7)]
        //public void GetPointOnPathFirstTest1(int ind, float expectedX)
        //{
        //    PathPoint pathPoints = PathPoint.CreateFromPoints(new List<GpsCoordinate>()
        //    {
        //        new GpsCoordinate() { x = 5, y = 5 },
        //        new GpsCoordinate() { x = 7, y = 5 }
        //    }, 0);

        //    var point = PointUtils.GetPointOnPath(pathPoints, ind);

        //    Assert.AreEqual(expectedX, point.Point.x);
        //}

        [Test]
        public void SplitPointsNullTest1()
        {
            Assert.IsNull(PointUtils.SplitPoints(null));
        }

        [Test]
        public void SplitPointsEmptyTest1()
        {
            Assert.AreEqual(0, PointUtils.SplitPoints(new List<GpsCoordinate>()).Count);
        }

        [Test]
        public void SplitPointsNoNullsTest1()
        {
            Assert.AreEqual(1, PointUtils.SplitPoints(
                new List<GpsCoordinate>()
                {
                    new GpsCoordinate()
                }).Count);
        }

        [Test]
        public void SplitPointsHasNullsTest1()
        {
            var res = (List<List<GpsCoordinate>>)PointUtils.SplitPoints(
                new List<GpsCoordinate>()
                {
                    null,
                    new GpsCoordinate(),
                    null,
                    null,
                    new GpsCoordinate()
                });

            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(1, res[0].Count);
            Assert.AreEqual(1, res[1].Count);
        }

        [Test]
        public void SplitPathDataAtIndexTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                        new GpsCoordinate(){ x = 0 },
                        new GpsCoordinate(){ x = 1 },
                        new GpsCoordinate(){ x = 2 },
                        new GpsCoordinate(){ x = 3 },
                }, 1);

            var tail = PointUtils.SplitPathDataAtIndex(pathsData[0], 2);

            Assert.AreEqual(2, tail.Points.Count);
            Assert.AreEqual(2, tail.Points[0].x);
            Assert.AreEqual(3, tail.Points[1].x);
        }

        [Test]
        public void SplitPathDataAtIndexIncorrectTest1()
        {
            var pathsData = PointUtils.CreateFromPoints(
                new List<GpsCoordinate>()
                {
                     new GpsCoordinate(){ x = 0 },
                     new GpsCoordinate(){ x = 1 },
                }, 1);

            Assert.IsNull(PointUtils.SplitPathDataAtIndex(null, 2));
            Assert.IsNull(PointUtils.SplitPathDataAtIndex(pathsData[0], -2));
            Assert.IsNull(PointUtils.SplitPathDataAtIndex(pathsData[0], 2));
        }
    }
}
