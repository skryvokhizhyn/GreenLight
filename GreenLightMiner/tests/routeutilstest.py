import unittest

from GreenLightMiner import route, routeutils
from GreenLightMiner.routecandidateinfo import RouteCandidateInfo


class RouteUtilsTest_remove_close_points(unittest.TestCase):
    def test_empty(self):
        rt = []
        routeutils.remove_close_points(rt, 10)

        self.assertEqual(0, len(rt))

    def test_one(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0)]
        routeutils.remove_close_points(rt, 10)

        self.assertEqual(1, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])

    def test_two_distant(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 100, 0, 0)]
        routeutils.remove_close_points(rt, 10)

        self.assertEqual(2, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])
        self.assertEqual(route.RouteXyzPoint(0, 100, 0, 0), rt[1])

    def test_two_close(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 10, 0, 0)]
        routeutils.remove_close_points(rt, 100)

        self.assertEqual(2, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])
        self.assertEqual(route.RouteXyzPoint(0, 10, 0, 0), rt[1])

    def test_three_close(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 10, 0, 0), route.RouteXyzPoint(0, 20, 0, 0)]
        routeutils.remove_close_points(rt, 100)

        self.assertEqual(2, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])
        self.assertEqual(route.RouteXyzPoint(0, 20, 0, 0), rt[1])

    def test_three_close_begin(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 10, 0, 0), route.RouteXyzPoint(0, 100, 0, 0)]
        routeutils.remove_close_points(rt, 20)

        self.assertEqual(3, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])
        self.assertEqual(route.RouteXyzPoint(0, 10, 0, 0), rt[1])
        self.assertEqual(route.RouteXyzPoint(0, 100, 0, 0), rt[2])

    def test_three_close_end(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 90, 0, 0), route.RouteXyzPoint(0, 100, 0, 0)]
        routeutils.remove_close_points(rt, 20)

        self.assertEqual(3, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])
        self.assertEqual(route.RouteXyzPoint(0, 90, 0, 0), rt[1])
        self.assertEqual(route.RouteXyzPoint(0, 100, 0, 0), rt[2])

    def test_many_mid(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 30, 0, 0), route.RouteXyzPoint(0, 40, 0, 0), route.RouteXyzPoint(0, 50, 0, 0), route.RouteXyzPoint(0, 100, 0, 0)]
        routeutils.remove_close_points(rt, 25)

        self.assertEqual(4, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])
        self.assertEqual(route.RouteXyzPoint(0, 30, 0, 0), rt[1])
        self.assertEqual(route.RouteXyzPoint(0, 50, 0, 0), rt[2])
        self.assertEqual(route.RouteXyzPoint(0, 100, 0, 0), rt[3])

    def test_all_close(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 10, 0, 0), route.RouteXyzPoint(0, 20, 0, 0), route.RouteXyzPoint(0, 30, 0, 0), route.RouteXyzPoint(0, 40, 0, 0)]
        routeutils.remove_close_points(rt, 100)

        self.assertEqual(2, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])
        self.assertEqual(route.RouteXyzPoint(0, 40, 0, 0), rt[1])

    def test_two_points_between_first_kept(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 20, 0, 0), route.RouteXyzPoint(0, 22, 0, 0), route.RouteXyzPoint(0, 30, 0, 0)]
        routeutils.remove_close_points(rt, 25)

        self.assertEqual(3, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])
        self.assertEqual(route.RouteXyzPoint(0, 20, 0, 0), rt[1])
        self.assertEqual(route.RouteXyzPoint(0, 30, 0, 0), rt[2])

    def test_two_points_between_second_kept(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 5, 0, 0), route.RouteXyzPoint(0, 15, 0, 0), route.RouteXyzPoint(0, 30, 0, 0)]
        routeutils.remove_close_points(rt, 25)

        self.assertEqual(3, len(rt))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), rt[0])
        self.assertEqual(route.RouteXyzPoint(0, 15, 0, 0), rt[1])
        self.assertEqual(route.RouteXyzPoint(0, 30, 0, 0), rt[2])


class RouteUtilsTest_get_mid_point_index(unittest.TestCase):
    def test_wrong_or_close_indices(self):
        rt = [
            route.RouteXyzPoint(0, 1, 0, 0),
            route.RouteXyzPoint(0, 2, 0, 0),
            route.RouteXyzPoint(0, 3, 0, 0),
            route.RouteXyzPoint(0, 10, 0, 0)
        ]

        self.assertRaises(Exception, routeutils.get_mid_point_index, rt, 5, 2)
        self.assertRaises(Exception, routeutils.get_mid_point_index, rt, -1, 2)
        self.assertRaises(Exception, routeutils.get_mid_point_index, rt, -2, -1)
        self.assertIsNone(routeutils.get_mid_point_index(rt, 1, 2))
        self.assertIsNone(routeutils.get_mid_point_index(rt, 1, 1))

    def test_one_mid(self):
        rt = [
            route.RouteXyzPoint(0, 1, 0, 0),
            route.RouteXyzPoint(0, 2, 0, 0),
            route.RouteXyzPoint(0, 3, 0, 0),
            route.RouteXyzPoint(0, 10, 0, 0)
        ]

        self.assertEqual(2, routeutils.get_mid_point_index(rt, 1, 3))

    def test_close_to_end(self):
        rt = [
            route.RouteXyzPoint(0, 1, 0, 0),
            route.RouteXyzPoint(0, 2, 0, 0),
            route.RouteXyzPoint(0, 3, 0, 0),
            route.RouteXyzPoint(0, 10, 0, 0)
        ]

        self.assertEqual(2, routeutils.get_mid_point_index(rt, 0, 3))

    def test_close_to_begin(self):
        rt = [
            route.RouteXyzPoint(0, 1, 0, 0),
            route.RouteXyzPoint(0, 8, 0, 0),
            route.RouteXyzPoint(0, 9, 0, 0),
            route.RouteXyzPoint(0, 10, 0, 0)
        ]

        self.assertEqual(1, routeutils.get_mid_point_index(rt, 0, 3))

    def test_many_in_mid(self):
        rt = [
            route.RouteXyzPoint(0, 1, 0, 0),
            route.RouteXyzPoint(0, 2, 0, 0),
            route.RouteXyzPoint(0, 3, 0, 0),
            route.RouteXyzPoint(0, 4, 0, 0),
            route.RouteXyzPoint(0, 8, 0, 0),
            route.RouteXyzPoint(0, 9, 0, 0),
            route.RouteXyzPoint(0, 10, 0, 0)
        ]

        self.assertEqual(3, routeutils.get_mid_point_index(rt, 0, 6))


class RouteUtilsTest_get_length(unittest.TestCase):
    def test_emty_one(self):
        self.assertEqual(0, routeutils.get_length([]))
        self.assertEqual(0, routeutils.get_length([1]))

    def test_multiple_points(self):
        self.assertEqual(20, routeutils.get_length([
            route.RouteXyzPoint(0, 0, 0, 0),
            route.RouteXyzPoint(0, 10, 0, 0),
            route.RouteXyzPoint(0, 20, 0, 0)
        ]))


class RouteUtilsTest_split_by_time(unittest.TestCase):
    def test_empty(self):
        res = routeutils.split_by_time([], 5)
        self.assertEqual(1, len(res))
        self.assertEqual(0, len(res[0]))

    def test_one(self):
        self.assertEqual(1, len(routeutils.split_by_time([route.RouteXyzPoint(0, 0, 0, 0)], 5)))

    def test_all_in_one_chunk(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(1, 0, 0, 0), route.RouteXyzPoint(2, 0, 0, 0), route.RouteXyzPoint(3, 0, 0, 0)]
        res = routeutils.split_by_time(rt, 10)

        self.assertEqual(1, len(res))
        self.assertEqual(4, len(res[0]))

    def test_multiple(self):
        rt = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(10, 0, 0, 0), route.RouteXyzPoint(11, 0, 0, 0), route.RouteXyzPoint(20, 0, 0, 0)]
        res = routeutils.split_by_time(rt, 5)

        self.assertEqual(3, len(res))
        self.assertEqual(1, len(res[0]))
        self.assertEqual(route.RouteXyzPoint(0, 0, 0, 0), res[0][0])
        self.assertEqual(2, len(res[1]))
        self.assertEqual(route.RouteXyzPoint(10, 0, 0, 0), res[1][0])
        self.assertEqual(route.RouteXyzPoint(11, 0, 0, 0), res[1][1])
        self.assertEqual(1, len(res[2]))
        self.assertEqual(route.RouteXyzPoint(20, 0, 0, 0), res[2][0])


class RouteUtilsTest_advance_candidates(unittest.TestCase):
    def test_no_candidates(self):
        candidates = []
        routeutils.advance_candidates(route.RouteXyzPoint(0, 1, 0, 0), candidates, [], 1)
        self.assertEqual(0, len(candidates))

    def test_wrong_route_id(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0)]
        candidates = [RouteCandidateInfo(rt0[0], 0, 1)]

        self.assertRaises(Exception, routeutils.advance_candidates, route.RouteXyzPoint(0, 1.1, 0, 0), candidates, [rt0], 1)

    def test_wrong_point_id(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0)]
        candidates = [RouteCandidateInfo(rt0[0], 1, 0)]

        self.assertRaises(Exception, routeutils.advance_candidates, route.RouteXyzPoint(0, 1.1, 0, 0), candidates, [rt0], 1)

    def test_all_advanced(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 0, 0), route.RouteXyzPoint(0, 2, 0, 0), route.RouteXyzPoint(0, 3, 0, 0)]
        rt1: route.XyzRoute = [route.RouteXyzPoint(0, 0.1, 0, 0), route.RouteXyzPoint(0, 1.1, 0.1, 0), route.RouteXyzPoint(0, 2.1, 0.2, 0), route.RouteXyzPoint(0, 3.1, 0.3, 0)]
        candidates = [RouteCandidateInfo(rt0[1], 1, 0), RouteCandidateInfo(rt1[1], 1, 1)]

        routeutils.advance_candidates(route.RouteXyzPoint(0, 1.5, 0, 0), candidates, [rt0, rt1], 2)

        self.assertEqual(2, len(candidates))

    def test_one_advanced(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1.1, 0, 0), route.RouteXyzPoint(0, 2, 0, 0), route.RouteXyzPoint(0, 3, 0, 0)]
        rt1: route.XyzRoute = [route.RouteXyzPoint(0, 0.1, 0, 0), route.RouteXyzPoint(0, 0.9, 0.1, 0), route.RouteXyzPoint(0, 3.1, 0.2, 0), route.RouteXyzPoint(0, 4.1, 0.3, 0)]
        candidates = [RouteCandidateInfo(rt0[1], 1, 0), RouteCandidateInfo(rt1[1], 1, 1)]

        routeutils.advance_candidates(route.RouteXyzPoint(0, 1.7, 0, 0), candidates, [rt0, rt1], 0.6)

        self.assertEqual(1, len(candidates))

    def test_all_removed(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 0, 0), route.RouteXyzPoint(0, 2, 0, 0), route.RouteXyzPoint(0, 30, 0, 0)]
        rt1: route.XyzRoute = [route.RouteXyzPoint(0, 0.1, 0, 0), route.RouteXyzPoint(0, 1.1, 0.1, 0), route.RouteXyzPoint(0, 3.1, 0.2, 0), route.RouteXyzPoint(0, 40.1, 0.3, 0)]
        candidates = [RouteCandidateInfo(rt0[1], 1, 0), RouteCandidateInfo(rt1[1], 1, 1)]

        routeutils.advance_candidates(route.RouteXyzPoint(0, 10, 0, 0), candidates, [rt0, rt1], 0.6)

        self.assertEqual(0, len(candidates))


class RouteUtilsTest_remove_not_same_direction(unittest.TestCase):
    def test_no_candidates(self):
        candidates = []
        routeutils.remove_not_same_direction(route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 0, 0), candidates, [], 10)
        self.assertEqual(0, len(candidates))

    def test_candidate_id_0(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 0, 0), route.RouteXyzPoint(0, 2, 0, 0), route.RouteXyzPoint(0, 3, 0, 0)]
        rt1: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 1, 0), route.RouteXyzPoint(0, 2, 2, 0), route.RouteXyzPoint(0, 3, 3, 0)]
        candidates = [RouteCandidateInfo(rt0[0], 0, 0), RouteCandidateInfo(rt1[0], 0, 1)]

        routeutils.remove_not_same_direction(route.RouteXyzPoint(0, 0.1, 0, 0), route.RouteXyzPoint(0, 1.1, 0, 0), candidates, [rt0, rt1], 20)

        self.assertEqual(2, len(candidates))

    def test_wrong_route_id(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0)]
        candidates = [RouteCandidateInfo(rt0[0], 0, 1)]

        self.assertRaises(Exception, routeutils.remove_not_same_direction, route.RouteXyzPoint(0, 0.1, 0, 0), route.RouteXyzPoint(0, 1.1, 0, 0), candidates, [rt0], 20)

    def test_wrong_point_id(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0)]
        candidates = [RouteCandidateInfo(rt0[0], 1, 0)]

        self.assertRaises(Exception, routeutils.remove_not_same_direction, route.RouteXyzPoint(0, 0.1, 0, 0), route.RouteXyzPoint(0, 1.1, 0, 0), candidates, [rt0], 20)

    def test_all_same_direction(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 0, 0), route.RouteXyzPoint(0, 2, 0, 0), route.RouteXyzPoint(0, 3, 0, 0)]
        rt1: route.XyzRoute = [route.RouteXyzPoint(0, 0.1, 0, 0), route.RouteXyzPoint(0, 1.1, 0.1, 0), route.RouteXyzPoint(0, 2.1, 0.2, 0), route.RouteXyzPoint(0, 3.1, 0.3, 0)]
        candidates = [RouteCandidateInfo(rt0[0], 1, 0), RouteCandidateInfo(rt1[1], 1, 1)]

        routeutils.remove_not_same_direction(route.RouteXyzPoint(0, 0.2, 0, 0), route.RouteXyzPoint(0, 1.2, 0, 0), candidates, [rt0, rt1], 20)

        self.assertEqual(2, len(candidates))

    def test_some_wrong_direction(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 0, 0), route.RouteXyzPoint(0, 2, 0, 0), route.RouteXyzPoint(0, 3, 0, 0)]
        rt1: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 1, 0), route.RouteXyzPoint(0, 2, 2, 0), route.RouteXyzPoint(0, 3, 3, 0)]
        candidates = [RouteCandidateInfo(rt0[0], 1, 0), RouteCandidateInfo(rt1[0], 1, 1)]

        routeutils.remove_not_same_direction(route.RouteXyzPoint(0, 0.1, 1, 0), route.RouteXyzPoint(0, 1.1, 2, 0), candidates, [rt0, rt1], 20)

        self.assertEqual(1, len(candidates))

    def test_all_wrong_direction(self):
        rt0: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 0, 0), route.RouteXyzPoint(0, 2, 0, 0), route.RouteXyzPoint(0, 3, 0, 0)]
        rt1: route.XyzRoute = [route.RouteXyzPoint(0, 0, 0, 0), route.RouteXyzPoint(0, 1, 1, 0), route.RouteXyzPoint(0, 2, 2, 0), route.RouteXyzPoint(0, 3, 3, 0)]
        candidates = [RouteCandidateInfo(rt0[0], 1, 0), RouteCandidateInfo(rt1[0], 1, 1)]

        routeutils.remove_not_same_direction(route.RouteXyzPoint(0, 2, 1, 0), route.RouteXyzPoint(0, 1.1, 2, 0), candidates, [rt0, rt1], 20)

        self.assertEqual(0, len(candidates))
