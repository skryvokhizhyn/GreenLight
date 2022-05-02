from typing import List, Dict

import routeutils
import utils
from routecancidateinfolocator import RouteCandidateInfoLocator
from routecandidateinfo import RouteCandidateInfo
from pointxyz import PointXyz, XyzRoute, XyzRoutes


class RouteAggregator:
    def __init__(self, tolerance_dist: float, tolerance_angle: float, min_allowed_points_count: int = 10):
        self.__tolerance = tolerance_dist
        self.__codirection_angle_degrees = tolerance_angle
        self.__min_allowed_points_count = min_allowed_points_count
        self.__routes: XyzRoutes = []
        self.__locator = RouteCandidateInfoLocator(self.__tolerance * 10)

    def aggregate_routes(self, rts: XyzRoutes) -> XyzRoutes:
        for rt in rts:
            self.__consume_route(rt)
        
        return self.__routes

    # doesn't aggregate co-directional routes and ignores points
    def __consume_route(self, rt: XyzRoute) -> None:
        candidates: List[RouteCandidateInfo] = []
        buffered_routes: XyzRoutes= []

        prev_filtered: int = -1

        for i in range(len(rt)):
            routeutils.advance_candidates(rt[i], candidates, self.__routes, self.__tolerance)

            if len(candidates) == 0:
                candidates = self.__get_closest_points(rt[i], self.__tolerance)

                if i > 0:
                    routeutils.remove_not_same_direction(rt[i - 1], rt[i], candidates, self.__routes, self.__codirection_angle_degrees)

            if len(candidates) > 0:
                p1: int = prev_filtered

                if p1 == -1:
                    p1 = 0

                if i - p1 > 1:
                    buffered_routes.append(rt[p1:i+1])

                prev_filtered = i

        if len(rt) - prev_filtered > 1:
            p2: int = prev_filtered

            if p2 == -1:
                p2 = 0

            buffered_routes.append(rt[p2:len(rt)])

        for r in buffered_routes:
            self.__add_route(r)

    def __get_closest_points(self, pt: PointXyz, tolerance: float) -> List[RouteCandidateInfo]:
        pts: List[RouteCandidateInfo] = []
        for info in self.__locator.get(pt, tolerance):
            pts.append(info)

        return pts

    def __add_route(self, rt: XyzRoute) -> None:
        if (len(rt) < self.__min_allowed_points_count):
            return

        i = 0
        for p in rt:
            self.__locator.put(RouteCandidateInfo(p, i, len(self.__routes)))
            i += 1

        self.__routes.append(rt.copy())
