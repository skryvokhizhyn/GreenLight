import unittest

from routecrossroadsplitter import RouteCrossroadSplitter
from pointxyz import PointXyz


class RouteCrossroadSplitterTest(unittest.TestCase):
    def test_one(self):
        routes = [[PointXyz(0, 0, 0), PointXyz(1, 0, 0)]]

        res = RouteCrossroadSplitter(10, 0.1).split(routes)

        self.assertEqual(1, len(res))
        self.assertEquals(routes, res)

    def test_two_intersecting(self):
        routes = [
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0)],
            [PointXyz(2, -2, 0), PointXyz(2, -1, 0), PointXyz(2, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)]]

        res = RouteCrossroadSplitter(10, 0.1).split(routes)

        self.assertEqual(2, len(res))
        self.assertEquals(routes[0], res[0])
        self.assertEquals(routes[1], res[1])

    def test_crossroad_first_split(self):
        routes = [
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0)],
            [PointXyz(2, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)]]

        res = RouteCrossroadSplitter(10, 0.1).split(routes)

        self.assertEqual(3, len(res))
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)], res[0])
        self.assertEquals([PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0)], res[1])
        self.assertEquals([PointXyz(2, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)], res[2])

    def test_crossroad_second_split(self):
        routes = [
            [PointXyz(2, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)],
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0)]]

        res = RouteCrossroadSplitter(10, 0.1).split(routes)

        self.assertEqual(3, len(res))
        self.assertEquals([PointXyz(2, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)], res[0])
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)], res[1])
        self.assertEquals([PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0)], res[2])

    def test_first_road_split_twice_by_one(self):
        routes = [
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0)],
            [PointXyz(1, 0, 0), PointXyz(1, 1, 0), PointXyz(2, 1, 0), PointXyz(3, 1, 0), PointXyz(3, 0, 0)]]

        res = RouteCrossroadSplitter(10, 0.1).split(routes)

        self.assertEqual(4, len(res))
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0)], res[0])
        self.assertEquals([PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0)], res[1])
        self.assertEquals([PointXyz(3, 0, 0), PointXyz(4, 0, 0)], res[2])
        self.assertEquals([PointXyz(1, 0, 0), PointXyz(1, 1, 0), PointXyz(2, 1, 0), PointXyz(3, 1, 0), PointXyz(3, 0, 0)], res[3])

    def test_second_road_split_twice_by_one(self):
        routes = [
            [PointXyz(1, 0, 0), PointXyz(1, 1, 0), PointXyz(2, 1, 0), PointXyz(3, 1, 0), PointXyz(3, 0, 0)],
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0)]]

        res = RouteCrossroadSplitter(10, 0.1).split(routes)

        self.assertEqual(4, len(res))
        self.assertEquals([PointXyz(1, 0, 0), PointXyz(1, 1, 0), PointXyz(2, 1, 0), PointXyz(3, 1, 0), PointXyz(3, 0, 0)], res[0])
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0)], res[1])
        self.assertEquals([PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0)], res[2])
        self.assertEquals([PointXyz(3, 0, 0), PointXyz(4, 0, 0)], res[3])

    def test_trow_roads_split_at_one_point_by_third(self):
        routes = [
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0)],
            [PointXyz(2, -2, 0), PointXyz(2, -1, 0), PointXyz(2, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)],
            [PointXyz(2, 0, 0), PointXyz(3, 1, 0)]
            ]

        res = RouteCrossroadSplitter(10, 0.1).split(routes)

        self.assertEqual(5, len(res))
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)], res[0])
        self.assertEquals([PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0)], res[1])
        self.assertEquals([PointXyz(2, -2, 0), PointXyz(2, -1, 0), PointXyz(2, 0, 0)], res[2])
        self.assertEquals([PointXyz(2, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)], res[3])
        self.assertEquals([PointXyz(2, 0, 0), PointXyz(3, 1, 0)], res[4])
        
        