import unittest

from GreenLightMiner import routeutils


class RouteUtilsTest(unittest.TestCase):
    def test_remove_close_points_empty(self):
        self.assertEqual(0, len(routeutils.remove_close_points([], 10)))
        self.assertFalse(True)
