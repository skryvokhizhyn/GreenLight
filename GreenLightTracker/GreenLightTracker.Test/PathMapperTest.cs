using System.Collections.Generic;
using NUnit.Framework;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    [TestFixture]
    class PathMapperTest
    {
        [TestCase(10, 10, 0, 0)]
        [TestCase(110, 110, 100, 100)]
        [TestCase(-110, -110, -200, -200)]
        public void CellByPointTest(float givenX, float givenY, float expectedX, float expectedY)
        {
            var cell = PathMapper.GetCellByPoint(new GpsCoordinate() { x = givenX, y = givenY, z = 10 });

            Assert.AreEqual(expectedX, cell.x);
            Assert.AreEqual(expectedY, cell.y);
        }

        [TestCase(50, 50, 1)]
        [TestCase(190, 150, 1)]
        [TestCase(150, 190, 1)]
        [TestCase(-190, -150, 1)]
        [TestCase(-150, -190, 1)]
        [TestCase(100, 150, 2)]
        [TestCase(150, 100, 2)]
        [TestCase(191, 150, 2)]
        [TestCase(150, 191, 2)]
        [TestCase(-100, -50, 2)]
        [TestCase(-150, -100, 2)]
        [TestCase(-191, -150, 2)]
        [TestCase(-150, -191, 2)]
        [TestCase(205, 205, 3)]
        [TestCase(295, 295, 3)]
        [TestCase(205, 295, 3)]
        [TestCase(-295, -205, 3)]
        [TestCase(-205, -205, 3)]
        [TestCase(-295, -295, 3)]
        [TestCase(-205, -295, 3)]
        public void GetTouchedCellsTest(float givenX, float givenY, int expetedCount)
        {
            var cells = PathMapper.GetTouchedCells(new GpsCoordinate() { x = givenX, y = givenY, z = 10 }, 10.0f);

            Assert.AreEqual(expetedCount, cells.Count);
        }

        [Test]
        public void GetTouchedCellsPositiveValueTest()
        {
            var cells = PathMapper.GetTouchedCells(new GpsCoordinate() { x = 205, y = 295, z = 10 }, 10.0f);

            Assert.AreEqual(3, cells.Count);
            Assert.AreEqual(200, cells[0].x);
            Assert.AreEqual(200, cells[0].y);
            Assert.AreEqual(100, cells[1].x);
            Assert.AreEqual(200, cells[1].y);
            Assert.AreEqual(200, cells[2].x);
            Assert.AreEqual(300, cells[2].y);
        }

        [Test]
        public void GetTouchedCellsNegativeValueTest()
        {
            var cells = PathMapper.GetTouchedCells(new GpsCoordinate() { x = -205, y = -295, z = 10 }, 10.0f);

            Assert.AreEqual(3, cells.Count);
            Assert.AreEqual(-300, cells[0].x);
            Assert.AreEqual(-300, cells[0].y);
            Assert.AreEqual(-300, cells[1].x);
            Assert.AreEqual(-400, cells[1].y);
            Assert.AreEqual(-200, cells[2].x);
            Assert.AreEqual(-300, cells[2].y);
        }

        //[Test]
        //public void GetNeighborsTest()
        //{
        //    var mapper = new PathMapper(10.0f);

        //    var p1 = new GpsCoordinate() { x = 5, y = 5, z = 0 };
        //    var p2 = new GpsCoordinate() { x = 50, y = 50, z = 0 };

        //    mapper.PutPoints(PathPoint.CreateFromPoints(new List<GpsCoordinate> { p1, p2 }, 0));

        //    var neighbors1 = mapper.GetNearestPoints(new GpsCoordinate() { x = 55, y = 55, z = 10 });

        //    Assert.AreEqual(2, neighbors1.Count);
        //    Assert.AreEqual(p1.x, neighbors1[0].Point.x);
        //    Assert.AreEqual(p1.y, neighbors1[0].Point.y);
        //    Assert.AreEqual(p2.x, neighbors1[1].Point.x);
        //    Assert.AreEqual(p2.y, neighbors1[1].Point.y);

        //    var neighbors2 = mapper.GetNearestPoints(new GpsCoordinate() { x = -55, y = -55, z = 10 });

        //    Assert.AreEqual(null, neighbors2);

        //    var neighbors3 = mapper.GetNearestPoints(new GpsCoordinate() { x = -55, y = 5, z = 10 });

        //    Assert.AreEqual(1, neighbors3.Count);
        //    Assert.AreEqual(p1.x, neighbors1[0].Point.x);
        //    Assert.AreEqual(p1.y, neighbors1[0].Point.y);

        //    var neighbors4 = mapper.GetNearestPoints(new GpsCoordinate() { x = 5, y = -55, z = 10 });

        //    Assert.AreEqual(1, neighbors3.Count);
        //    Assert.AreEqual(p2.x, neighbors1[1].Point.x);
        //    Assert.AreEqual(p2.y, neighbors1[1].Point.y);
        //}

        //[Test]
        //public void PathLinkingTest()
        //{
        //    var mapper = new PathMapper(10.0f);

        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            new GpsCoordinate() { x = 5, y = 5, z = 0 },
        //            new GpsCoordinate() { x = 7, y = 5, z = 0 },
        //        }, 0));
        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            new GpsCoordinate() { x = 10, y = 100, z = 0 }
        //        }, 1));

        //    var neighbors = mapper.GetNearestPoints(new GpsCoordinate() { x = 10, y = 10, z = 10 });

        //    Assert.AreEqual(3, neighbors.Count);
        //    Assert.AreEqual(neighbors[1], neighbors[0].Next);
        //    Assert.AreEqual(null, neighbors[1].Next);
        //    Assert.AreEqual(null, neighbors[2].Next);
        //}

        //[Test]
        //public void GetNeighborsWithinToleranceTest()
        //{
        //    var mapper = new PathMapper(10.0f);

        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            new GpsCoordinate() { x = 5, y = 5, z = 0 },
        //            new GpsCoordinate() { x = 7, y = 5, z = 0 },
        //        }, 0));
        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            new GpsCoordinate() { x = 10, y = 100, z = 0 },
        //        }, 1));
        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            new GpsCoordinate() { x = 6, y = 5, z = 0 },
        //            new GpsCoordinate() { x = 8, y = 5, z = 0 },
        //        }, 2));
        //    mapper.PutPoints(
        //       PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //       {
        //           new GpsCoordinate() { x = 11, y = 100, z = 0 }
        //       }, 3));

        //    var neighbors = mapper.GetNearestPointsFiltered(new GpsCoordinate() { x = 10, y = 10, z = 0 });

        //    Assert.AreEqual(2, neighbors.Count);
        //    Assert.AreEqual(7, neighbors[0].Point.x);
        //    Assert.AreEqual(5, neighbors[0].Point.y);
        //    Assert.AreEqual(8, neighbors[1].Point.x);
        //    Assert.AreEqual(5, neighbors[1].Point.y);
        //}

        //[Test]
        //public void PathMapperGetRoadsTest1()
        //{
        //    var mapper = new PathMapper(10.0f);

        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            new GpsCoordinate() { x = 5 },
        //            new GpsCoordinate() { x = 7 },
        //        }, 0));
        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            new GpsCoordinate() { x = 100 },
        //            new GpsCoordinate() { x = 105 },
        //        }, 1));

        //    var points = (IList<GpsCoordinate>)mapper.GetPoints();

        //    Assert.AreEqual(4, points.Count);
        //    Assert.AreEqual(5, points[0].x);
        //    Assert.AreEqual(7, points[1].x);
        //    Assert.AreEqual(100, points[2].x);
        //    Assert.AreEqual(105, points[3].x);
        //}

        //[Test]
        //public void PathMapperIsRoadStartTest1()
        //{
        //    var mapper = new PathMapper();

        //    var p1 = new GpsCoordinate() { x = 5 };
        //    var p2 = new GpsCoordinate() { x = 7 };

        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            p1, p2
        //        }, 0));
        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            new GpsCoordinate() { x = 100 },
        //            new GpsCoordinate() { x = 105 },
        //        }, 1));

        //    Assert.IsTrue(mapper.IsRoadStart(p1));
        //    Assert.IsFalse(mapper.IsRoadStart(p2));
        //    Assert.IsTrue(mapper.IsRoadStart(new GpsCoordinate() { x = 5 }));
        //    Assert.IsFalse(mapper.IsRoadStart(new GpsCoordinate() { x = 7 }));
        //    Assert.IsFalse(mapper.IsRoadStart(new GpsCoordinate() { x = 9 }));

        //    Assert.IsTrue(mapper.IsRoadStart(new GpsCoordinate() { x = 100 }));
        //}

        //[Test]
        //public void PathMapperAttachBeforePathPointTest1()
        //{
        //    var mapper = new PathMapper();

        //    mapper.PutPoints(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>
        //        {
        //            new GpsCoordinate() { x = 100 },
        //            new GpsCoordinate() { x = 105 },
        //        }, 0));

        //    var pathPoints = mapper.GetNearestPointsFiltered(new GpsCoordinate() { x = 95 });
        //    Assert.NotNull(pathPoints);
        //    Assert.IsTrue(pathPoints.Count > 0);

        //    mapper.AttachBeforePathPoint(
        //        PathPoint.CreateFromPoints(new List<GpsCoordinate>()
        //        {
        //            new GpsCoordinate(){ x = 1 },
        //            new GpsCoordinate(){ x = 2 }
        //        }, 1),
        //        ((List<PathPoint>)pathPoints)[0]);

        //    var points = (List<GpsCoordinate>)mapper.GetPoints();

        //    Assert.AreEqual(1, points[0].x);
        //    Assert.AreEqual(2, points[1].x);
        //    Assert.AreEqual(100, points[2].x);
        //    Assert.AreEqual(105, points[3].x);
        //}
    }
}
