import unittest

from routeaggregator import RouteAggregator
from pointxyz import PointXyz, XyzRoute, XyzRoutes


class RouteAggregatorTest_aggregate_routes(unittest.TestCase):
    def test_one(self):
        aggregator = RouteAggregator(1, 1, 2)
        routes: XyzRoutes = aggregator.aggregate_routes([[PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)]])
        self.assertEqual(1, len(routes))

    def test_clone(self):
        aggregator = RouteAggregator(1, 1, min_allowed_points_count = 2)
        rt = [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)]
        routes: XyzRoutes = aggregator.aggregate_routes([rt])
        self.assertFalse(routes[0] is rt)

    def test_covered(self):
        aggregator = RouteAggregator(1, 1, 2)
        routes: XyzRoutes = aggregator.aggregate_routes([
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)],
            [PointXyz(0.5, 0, 0), PointXyz(1.5, 0, 0)]])
        self.assertEqual(1, len(routes))

    def test_distant(self):
        aggregator = RouteAggregator(1, 1, 2)
        routes: XyzRoutes = aggregator.aggregate_routes([
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)],
            [PointXyz(5, 0, 0), PointXyz(6, 0, 0)]])
        self.assertEqual(2, len(routes))

    def test_add_route_small(self):
        aggregator = RouteAggregator(1, 1)
        aggregator._RouteAggregator__add_route([PointXyz(5, 0, 0)])
        self.assertEqual(0, len(aggregator._RouteAggregator__routes))

    def test_one_route_after_second_no_points_removed(self):
        aggregator = RouteAggregator(1, 1, 2)
        routes: XyzRoutes = aggregator.aggregate_routes([
            [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)],
            [PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)]])

        self.assertEqual(2, len(routes))
        self.assertEqual(3, len(routes[0]))
        self.assertEqual(3, len(routes[1]))
        