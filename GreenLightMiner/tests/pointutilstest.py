import math
import unittest

import pointutils
import route


class PointUtilsTest(unittest.TestCase):
    def test_distance(self):
        self.assertEqual(10, pointutils.distance(route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 10, 0, 0)))
        self.assertEqual(10, pointutils.distance(route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 0, 10, 0)))
        self.assertEqual(10, pointutils.distance(route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 0, 0, 10)))
        self.assertEqual(math.sqrt(300), pointutils.distance(route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 10, 10, 10)))
