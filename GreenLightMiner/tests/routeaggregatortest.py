import unittest

from routeaggregator import RouteAggregator
from route import RouteXyzPoint


class RouteAggregatorTest(unittest.TestCase):
    def test_empty(self):
        aggregator = RouteAggregator(1, 1)
        self.assertEqual(0, len(aggregator.routes))

    def test_one(self):
        aggregator = RouteAggregator(1, 1)
        aggregator.consume_route([RouteXyzPoint(0, 0, 0, 0), RouteXyzPoint(0, 1, 0, 0), RouteXyzPoint(0, 2, 0, 0)])
        self.assertEqual(1, len(aggregator.routes))

    def test_clone(self):
        aggregator = RouteAggregator(1, 1)
        rt = [RouteXyzPoint(0, 0, 0, 0), RouteXyzPoint(0, 1, 0, 0), RouteXyzPoint(0, 2, 0, 0)]
        aggregator.consume_route(rt)
        self.assertNotEqual(rt, aggregator.routes)

    def test_covered(self):
        aggregator = RouteAggregator(1, 1)
        aggregator.consume_route([RouteXyzPoint(0, 0, 0, 0), RouteXyzPoint(0, 1, 0, 0), RouteXyzPoint(0, 2, 0, 0)])
        aggregator.consume_route([RouteXyzPoint(0, 0.5, 0, 0), RouteXyzPoint(0, 1.5, 0, 0)])
        self.assertEqual(1, len(aggregator.routes))

    def test_distant(self):
        aggregator = RouteAggregator(1, 1)
        aggregator.consume_route([RouteXyzPoint(0, 0, 0, 0), RouteXyzPoint(0, 1, 0, 0), RouteXyzPoint(0, 2, 0, 0)])
        aggregator.consume_route([RouteXyzPoint(0, 5, 0, 0), RouteXyzPoint(0, 6, 0, 0)])
        self.assertEqual(2, len(aggregator.routes))

    def test_add_route_small(self):
        aggregator = RouteAggregator(1, 1)
        aggregator._RouteAggregator__add_route([RouteXyzPoint(0, 5, 0, 0)])
        self.assertEqual(0, len(aggregator.routes))

    def test_add_routes(self):
        aggregator = RouteAggregator(1, 1)
        rt0 = [RouteXyzPoint(0, 5, 0, 0), RouteXyzPoint(0, 6, 0, 0)]
        rt1 = [RouteXyzPoint(0, 0, 0, 0), RouteXyzPoint(0, 1, 0, 0), RouteXyzPoint(0, 2, 0, 0)]
        aggregator._RouteAggregator__add_route(rt0)
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

    def test_get_cloasest_points(self):
        aggregator = RouteAggregator(1, 1)
        rt0 = [RouteXyzPoint(0, -5, 0, 0), RouteXyzPoint(0, 5, 0, 0), RouteXyzPoint(0, 7, 0, 0)]
        rt1 = [RouteXyzPoint(0, 0, 0, 0), RouteXyzPoint(0, 1, 0, 0), RouteXyzPoint(0, 2, 0, 0)]
        aggregator._RouteAggregator__add_route(rt0)
        aggregator._RouteAggregator__add_route(rt1)

        infos = aggregator._RouteAggregator__get_closest_points(RouteXyzPoint(0, 3, 0, 0), 2)

        self.assertEqual(3, len(infos))

        self.assertEqual(RouteXyzPoint(0, 5, 0, 0), infos[0].point)
        self.assertEqual(1, infos[0].point_id)
        self.assertEqual(0, infos[0].route_id)

        self.assertEqual(RouteXyzPoint(0, 1, 0, 0), infos[1].point)
        self.assertEqual(1, infos[1].point_id)
        self.assertEqual(1, infos[1].route_id)

        self.assertEqual(RouteXyzPoint(0, 2, 0, 0), infos[2].point)
        self.assertEqual(2, infos[2].point_id)
        self.assertEqual(1, infos[2].route_id)
