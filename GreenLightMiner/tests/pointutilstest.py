import math
import unittest

import pointutils
from direction2d import Direction2d
from pointxyz import PointXyz


class PointUtilsTest(unittest.TestCase):
    def test_distance(self):
        self.assertEqual(10, pointutils.distance(PointXyz(0, 0, 0), PointXyz(10, 0, 0)))
        self.assertEqual(10, pointutils.distance(PointXyz(0, 0, 0), PointXyz(0, 10, 0)))
        self.assertEqual(10, pointutils.distance(PointXyz(0, 0, 0), PointXyz(0, 0, 10)))
        self.assertEqual(math.sqrt(300), pointutils.distance(PointXyz(0, 0, 0), PointXyz(10, 10, 10)))

    def test_get_angle_between(self):
        self.assertRaises(Exception, pointutils.get_angle_between, None, None)
        self.assertRaises(Exception, pointutils.get_angle_between, None, Direction2d(1, 1))
        self.assertRaises(Exception, pointutils.get_angle_between, Direction2d(1, 1), None)
        self.assertRaises(Exception, pointutils.get_angle_between, Direction2d(), Direction2d(1, 1))
        self.assertRaises(Exception, pointutils.get_angle_between, Direction2d(1, 1), Direction2d())

        self.assertEqual(90, pointutils.get_angle_between(Direction2d(1, 0), Direction2d(0, 1)))
        self.assertEqual(90, pointutils.get_angle_between(Direction2d(0, 1), Direction2d(1, 0)))
        self.assertEqual(0, pointutils.get_angle_between(Direction2d(0, 1), Direction2d(0, 1)))
        self.assertEqual(135, pointutils.get_angle_between(Direction2d(100, 0), Direction2d(-math.cos(math.radians(45)), math.cos(math.radians(45)))))

    def test_same_direction(self):
        self.assertTrue(pointutils.check_same_direction(Direction2d(1, 0), Direction2d(1, 0), 1))
        self.assertFalse(pointutils.check_same_direction(Direction2d(1, 0), Direction2d(0, 1), 45))
        self.assertTrue(pointutils.check_same_direction(Direction2d(1, 0), Direction2d(0, 1), 180))


class Direction2dTest(unittest.TestCase):
    def test_both_zero(self):
        self.assertFalse(Direction2d(0, 0).is_valid())
        self.assertFalse(Direction2d(9e-7, 9e-7).is_valid())
        self.assertFalse(Direction2d(-9e-7, -9e-7).is_valid())
        self.assertFalse(Direction2d(-9e-7, 9e-7).is_valid())
        self.assertFalse(Direction2d(9e-7, -9e-7).is_valid())

    def test_valid(self):
        self.assertTrue(Direction2d(-9e-7, 1).is_valid())
        self.assertTrue(Direction2d(1, -9e-7).is_valid())
        self.assertTrue(Direction2d(-9e-7, -1).is_valid())
        self.assertTrue(Direction2d(-1, -9e-7).is_valid())
