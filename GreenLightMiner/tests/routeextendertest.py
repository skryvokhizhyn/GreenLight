 
import unittest

from routeextender import RouteExtender
from pointxyz import PointXyz, XyzRoute, XyzRoutes

class RouteextenderTest_extend(unittest.TestCase):
    def test_empty(self):
        routes: XyzRoutes = []
        extender = RouteExtender(1)
        extender.extend(routes)
        self.assertEqual(0, len(routes))

    def test_one(self):
        routes: XyzRoutes = [[PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)]]
        extender = RouteExtender(1)
        extender.extend(routes)
        self.assertEqual(1, len(routes))

    def test_separate_routes(self):
        routes: XyzRoutes = [[PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)], [PointXyz(10, 0, 0), PointXyz(11, 0, 0), PointXyz(12, 0, 0)]]
        extender = RouteExtender(1)
        extender.extend(routes)
        self.assertEqual(2, len(routes))

    def test_two_joined_to_one_after(self):
        routes: XyzRoutes = [
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)], 
            [PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)]]
        extender = RouteExtender(1)
        result = extender.extend(routes)
        self.assertEqual(1, len(result))
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)], result[0])

    def test_two_joined_to_one_before(self):
        routes: XyzRoutes = [[PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)], [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)]]
        extender = RouteExtender(1)
        result = extender.extend(routes)
        self.assertEqual(1, len(result))
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)], result[0])

    def test_one_connects_two(self):
        routes: XyzRoutes = [
            [PointXyz(6, 0, 0), PointXyz(7, 0, 0), PointXyz(8, 0, 0)], 
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)], 
            [PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)]]
        extender = RouteExtender(1)
        result = extender.extend(routes)
        self.assertEqual(1, len(result))
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0),
                           PointXyz(5, 0, 0), PointXyz(6, 0, 0), PointXyz(7, 0, 0), PointXyz(8, 0, 0)], result[0])

    # def test_joins_in_middle_from(self):
    #     extender = RouteExtender(1, 1, 2)
    #     extender.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
    #     extender.consume_route([PointXyz(2.1, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)])
    #     self.assertEqual(2, len(routes))
    #     extender.extend()
    #     self.assertEqual(2, len(routes))

    # def test_joins_in_middle_into(self):
    #     extender = RouteExtender(1, 1, 2)
    #     extender.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
    #     extender.consume_route([PointXyz(2.1, -2, 0), PointXyz(2, -1, 0), PointXyz(2.1, 0, 0)])
    #     self.assertEqual(2, len(routes))
    #     extender.extend()
    #     self.assertEqual(2, len(routes))

    # def test_one_separate_one_extended_end(self):
    #     extender = RouteExtender(1, 1, 2)
    #     extender.consume_route([PointXyz(16, 0, 0), PointXyz(17, 0, 0), PointXyz(18, 0, 0)])
    #     extender.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
    #     extender.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
    #     self.assertEqual(3, len(routes))
    #     extender.extend()
    #     self.assertEqual(2, len(routes))

    # def test_one_separate_one_extended_beg(self):
    #     extender = RouteExtender(1, 1, 2)
    #     extender.consume_route([PointXyz(16, 0, 0), PointXyz(17, 0, 0), PointXyz(18, 0, 0)])
    #     extender.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
    #     extender.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
    #     self.assertEqual(3, len(routes))
    #     extender.extend()
    #     self.assertEqual(2, len(routes))
