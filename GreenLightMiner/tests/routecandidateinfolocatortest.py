import unittest
from GreenLightMiner.routecancidateinfolocator import RouteCandidateInfoLocator, RouteCandidateInfo
from GreenLightMiner.pointxyz import PointXyz


class RouteCandidateInfoLocatorTest(unittest.TestCase):
    def test_get_within_distance(self):
        locator = RouteCandidateInfoLocator(100)
        locator.put(RouteCandidateInfo(PointXyz(11, 11, 0), 0, 0))
        locator.put(RouteCandidateInfo(PointXyz(12, 12, 0), 1, 1))

        res = locator.get(PointXyz(13, 12, 0), 1)
        self.assertEqual(1, len(res))
        self.assertEqual(PointXyz(12, 12, 0), res[0].point)
        self.assertEqual(1, res[0].point_id)
        self.assertEqual(1, res[0].route_id)
