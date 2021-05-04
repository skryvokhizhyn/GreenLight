import unittest

from routeaggregator import RouteAggregator
from pointxyz import PointXyz


class RouteAggregatorTest(unittest.TestCase):
    def test_empty(self):
        aggregator = RouteAggregator(1, 1)
        self.assertEqual(0, len(aggregator.routes))

    def test_one(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        self.assertEqual(1, len(aggregator.routes))

    def test_clone(self):
        aggregator = RouteAggregator(1, 1)
        rt = [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)]
        aggregator.consume_route(rt)
        self.assertNotEqual(rt, aggregator.routes)

    def test_covered(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        aggregator.consume_route([PointXyz(0.5, 0, 0), PointXyz(1.5, 0, 0)])
        self.assertEqual(1, len(aggregator.routes))

    def test_distant(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        aggregator.consume_route([PointXyz(5, 0, 0), PointXyz(6, 0, 0)])
        self.assertEqual(2, len(aggregator.routes))

    def test_add_route_small(self):
        aggregator = RouteAggregator(1, 1)
        aggregator._RouteAggregator__add_route([PointXyz(5, 0, 0)])
        self.assertEqual(0, len(aggregator.routes))

    def test_add_routes(self):
        aggregator = RouteAggregator(1, 1, 2)
        rt0 = [PointXyz(5, 0, 0), PointXyz(6, 0, 0)]
        rt1 = [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)]
        aggregator._RouteAggregator__add_route(rt0,)
        aggregator._RouteAggregator__add_route(rt1)

        values = list(aggregator._RouteAggregator__locator._RouteCandidateInfoLocator__grid._GridMap2d__grid.values())[0]

        self.assertEqual(5, len(values))

        self.assertEqual(rt0[0], values[0].point)
        self.assertEqual(0, values[0].point_id)
        self.assertEqual(0, values[0].route_id)

        self.assertEqual(rt0[1], values[1].point)
        self.assertEqual(1, values[1].point_id)
        self.assertEqual(0, values[1].route_id)

        self.assertEqual(rt1[0], values[2].point)
        self.assertEqual(0, values[2].point_id)
        self.assertEqual(1, values[2].route_id)

        self.assertEqual(rt1[1], values[3].point)
        self.assertEqual(1, values[3].point_id)
        self.assertEqual(1, values[3].route_id)

        self.assertEqual(rt1[2], values[4].point)
        self.assertEqual(2, values[4].point_id)
        self.assertEqual(1, values[4].route_id)

    def test_one_route_after_second_no_points_removed(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])

        self.assertEqual(2, len(aggregator.routes))
        self.assertEqual(3, len(aggregator.routes[0]))
        self.assertEqual(3, len(aggregator.routes[1]))

    def test_get_cloasest_points(self):
        aggregator = RouteAggregator(1, 1, 2)
        rt0 = [PointXyz(-5, 0, 0), PointXyz(5, 0, 0), PointXyz(7, 0, 0)]
        rt1 = [PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)]
        aggregator._RouteAggregator__add_route(rt0)
        aggregator._RouteAggregator__add_route(rt1)

        infos = aggregator._RouteAggregator__get_closest_points(PointXyz(3, 0, 0), 2)

        self.assertEqual(3, len(infos))

        self.assertEqual(PointXyz(5, 0, 0), infos[0].point)
        self.assertEqual(1, infos[0].point_id)
        self.assertEqual(0, infos[0].route_id)

        self.assertEqual(PointXyz(1, 0, 0), infos[1].point)
        self.assertEqual(1, infos[1].point_id)
        self.assertEqual(1, infos[1].route_id)

        self.assertEqual(PointXyz(2, 0, 0), infos[2].point)
        self.assertEqual(2, infos[2].point_id)
        self.assertEqual(1, infos[2].route_id)


class RouteAggregatorTest_extend_rotes(unittest.TestCase):
    def test_empty(self):
        aggregator = RouteAggregator(1, 1)
        aggregator.extend_routes()
        self.assertEqual(0, len(aggregator.routes))

    def test_one(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        aggregator.extend_routes()
        self.assertEqual(1, len(aggregator.routes))

    def test_separate_routes(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        aggregator.consume_route([PointXyz(10, 0, 0), PointXyz(11, 0, 0), PointXyz(12, 0, 0)])
        aggregator.extend_routes()
        self.assertEqual(2, len(aggregator.routes))

    def test_two_joined_to_one_after(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
        self.assertEqual(2, len(aggregator.routes))
        aggregator.extend_routes()
        self.assertEqual(1, len(aggregator.routes))
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)], aggregator.routes[0])

    def test_two_joined_to_one_before(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        self.assertEqual(2, len(aggregator.routes))
        aggregator.extend_routes()
        self.assertEqual(1, len(aggregator.routes))
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)], aggregator.routes[0])

    def test_one_connects_two(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(6, 0, 0), PointXyz(7, 0, 0), PointXyz(8, 0, 0)])
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
        self.assertEqual(3, len(aggregator.routes))
        aggregator.extend_routes()
        self.assertEqual(1, len(aggregator.routes))
        self.assertEquals([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0),
                           PointXyz(5, 0, 0), PointXyz(6, 0, 0), PointXyz(7, 0, 0), PointXyz(8, 0, 0)], aggregator.routes[0])

    def test_joins_in_middle_from(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
        aggregator.consume_route([PointXyz(2.1, 0, 0), PointXyz(2, 1, 0), PointXyz(2, 2, 0)])
        self.assertEqual(2, len(aggregator.routes))
        aggregator.extend_routes()
        self.assertEqual(2, len(aggregator.routes))

    def test_joins_in_middle_into(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0), PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
        aggregator.consume_route([PointXyz(2.1, -2, 0), PointXyz(2, -1, 0), PointXyz(2.1, 0, 0)])
        self.assertEqual(2, len(aggregator.routes))
        aggregator.extend_routes()
        self.assertEqual(2, len(aggregator.routes))

    def test_one_separate_one_extended_end(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(16, 0, 0), PointXyz(17, 0, 0), PointXyz(18, 0, 0)])
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
        self.assertEqual(3, len(aggregator.routes))
        aggregator.extend_routes()
        self.assertEqual(2, len(aggregator.routes))

    def test_one_separate_one_extended_beg(self):
        aggregator = RouteAggregator(1, 1, 2)
        aggregator.consume_route([PointXyz(16, 0, 0), PointXyz(17, 0, 0), PointXyz(18, 0, 0)])
        aggregator.consume_route([PointXyz(3, 0, 0), PointXyz(4, 0, 0), PointXyz(5, 0, 0)])
        aggregator.consume_route([PointXyz(0, 0, 0), PointXyz(1, 0, 0), PointXyz(2, 0, 0)])
        self.assertEqual(3, len(aggregator.routes))
        aggregator.extend_routes()
        self.assertEqual(2, len(aggregator.routes))
