import unittest
from GreenLightMiner.routecancidateinfolocator import RouteCandidateInfoLocator, RouteCandidateInfo
from GreenLightMiner.route import RouteXyzPoint


class RouteCandidateInfoLocatorTest(unittest.TestCase):
    def test_get_within_distance(self):
        locator = RouteCandidateInfoLocator(100)
        locator.put(RouteCandidateInfo(RouteXyzPoint(0, 11, 11, 0), 0, 0))
        locator.put(RouteCandidateInfo(RouteXyzPoint(0, 12, 12, 0), 1, 1))

        res = locator.get(RouteXyzPoint(0, 13, 12, 0), 1)
        self.assertEqual(1, len(res))
        self.assertEqual(RouteXyzPoint(0, 12, 12, 0), res[0].point)
        self.assertEqual(1, res[0].point_id)
        self.assertEqual(1, res[0].route_id)
