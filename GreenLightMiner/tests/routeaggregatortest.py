import unittest

from routeaggregator import RouteAggregator
from route import RouteXyzPoint


class RouteAggregatorTest(unittest.TestCase):
    def test_empty(self):
        aggregator = RouteAggregator()
        self.assertEqual(0, len(aggregator.routes))

    def test_one(self):
        aggregator = RouteAggregator()
        aggregator.consume_route([1, 2, 3])
        self.assertEqual(1, len(aggregator.routes))

    def test_clone(self):
        aggregator = RouteAggregator()
        rt = [1, 2, 3]
        aggregator.consume_route(rt)
        self.assertNotEqual(rt, aggregator.routes)

    def test_covered(self):
        aggregator = RouteAggregator()
        aggregator.consume_route([RouteXyzPoint(0, 0, 0, 0), RouteXyzPoint(0, 1, 0, 0), RouteXyzPoint(0, 2, 0, 0)])
        aggregator.consume_route([RouteXyzPoint(0, 0.5, 0, 0), RouteXyzPoint(0, 1.5, 0, 0)])
        self.assertEqual(1, len(aggregator.routes))

    def test_distant(self):
        aggregator = RouteAggregator()
        aggregator.consume_route([RouteXyzPoint(0, 0, 0, 0), RouteXyzPoint(0, 1, 0, 0), RouteXyzPoint(0, 2, 0, 0)])
        aggregator.consume_route([RouteXyzPoint(0, 5, 0, 0), RouteXyzPoint(0, 6, 0, 0)])
        self.assertEqual(2, len(aggregator.routes))
