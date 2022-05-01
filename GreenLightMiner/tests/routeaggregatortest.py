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
  
 
# class RouteAggregatorTest_extend_routes(unittest.TestCase):
#     def test_empty(self):
#         aggregator = RouteAggregator(1, 1)
#         aggregator.extend_routes()
#         self.assertEqual(0, len(aggregator.routes))

#     def test_one(self):
#         aggregator = RouteAggregator(1, 1, 2)
#         aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
#         aggregator.extend_routes()
#         self.assertEqual(1, len(aggregator.routes))

#     def test_separate_routes(self):
#         aggregator = RouteAggregator(1, 1, 2)
#         aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
#         aggregator.consume_route([PointXyz(10, 0, 0), PointXyz(11, 0, 0), PointXyz(12, 0, 0)])
#         aggregator.extend_routes()
#         self.assertEqual(2, len(aggregator.routes))

#     def test_two_joined_to_one_after(self):
#         aggregator = RouteAggregator(1, 1, 2)
#         aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
#         aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
#         self.assertEqual(2, len(aggregator.routes))
#         aggregator.extend_routes()
#         self.assertEqual(1, len(aggregator.routes))
#         self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)], aggregator.routes[0])

#     def test_two_joined_to_one_before(self):
#         aggregator = RouteAggregator(1, 1, 2)
#         aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
#         aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
#         self.assertEqual(2, len(aggregator.routes))
#         aggregator.extend_routes()
#         self.assertEqual(1, len(aggregator.routes))
#         self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)], aggregator.routes[0])

#     def test_one_connects_two(self):
#         aggregator = RouteAggregator(1, 1, 2)
#         aggregator.consume_route([PointXyz(6, 0, 0), PointXyz(7, 0, 0), PointXyz(8, 0, 0)])
#         aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
#         aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
#         self.assertEqual(3, len(aggregator.routes))
#         aggregator.extend_routes()
#         self.assertEqual(1, len(aggregator.routes))
#         self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0),
#                            PointXyz(5, 0, 0), PointXyz(6, 0, 0), PointXyz(7, 0, 0), PointXyz(8, 0, 0)], aggregator.routes[0])

#     def test_joins_in_middle_from(self):
#         aggregator = RouteAggregator(1, 1, 2)
#         aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
#         aggregator.consume_route([PointXyz(2.1, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)])
#         self.assertEqual(2, len(aggregator.routes))
#         aggregator.extend_routes()
#         self.assertEqual(2, len(aggregator.routes))

#     def test_joins_in_middle_into(self):
#         aggregator = RouteAggregator(1, 1, 2)
#         aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
#         aggregator.consume_route([PointXyz(2.1, -2, 0), PointXyz(2, -1, 0), PointXyz(2.1, 0, 0)])
#         self.assertEqual(2, len(aggregator.routes))
#         aggregator.extend_routes()
#         self.assertEqual(2, len(aggregator.routes))

#     def test_one_separate_one_extended_end(self):
#         aggregator = RouteAggregator(1, 1, 2)
#         aggregator.consume_route([PointXyz(16, 0, 0), PointXyz(17, 0, 0), PointXyz(18, 0, 0)])
#         aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
#         aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
#         self.assertEqual(3, len(aggregator.routes))
#         aggregator.extend_routes()
#         self.assertEqual(2, len(aggregator.routes))

#     def test_one_separate_one_extended_beg(self):
#         aggregator = RouteAggregator(1, 1, 2)
#         aggregator.consume_route([PointXyz(16, 0, 0), PointXyz(17, 0, 0), PointXyz(18, 0, 0)])
#         aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
#         aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
#         self.assertEqual(3, len(aggregator.routes))
#         aggregator.extend_routes()
#         self.assertEqual(2, len(aggregator.routes))
