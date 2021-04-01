from typing import List

import routeutils
from routecancidateinfolocator import RouteCandidateInfoLocator
from routecandidateinfo import RouteCandidateInfo
from pointxyz import PointXyz, PointXyzList


class RouteAggregator:
    def __init__(self, tolerance_dist: float, tolerance_angle: float):
        self.__tolerance = tolerance_dist
        self.__codirection_angle_degrees = tolerance_angle
        self.__routes: List[PointXyzList] = []
        self.__locator = RouteCandidateInfoLocator(self.__tolerance * 10)

    # doesn't aggregate co-directional routes and ignores points
    def consume_route(self, rt: PointXyzList) -> None:
        candidates: List[RouteCandidateInfo] = []
        buffered_routes: List[PointXyzList] = []

        prev_filtered: int = -1

        for i in range(len(rt)):
            routeutils.advance_candidates(rt[i], candidates, self.__routes, self.__tolerance)

            if len(candidates) == 0:
                candidates = self.__get_closest_points(rt[i], self.__tolerance)

                if i > 0:
                    routeutils.remove_not_same_direction(rt[i - 1], rt[i], candidates, self.__routes, self.__codirection_angle_degrees)

            if len(candidates) > 0:
                if i - prev_filtered > 1:
                    buffered_routes.append(rt[prev_filtered + 1:i])

                prev_filtered = i

        if len(rt) - prev_filtered > 1:
            buffered_routes.append(rt[prev_filtered + 1:len(rt)])

        for r in buffered_routes:
            self.__add_route(r)

    @ property
    def routes(self) -> List[PointXyzList]:
        return self.__routes

    def __get_closest_points(self, pt: PointXyz, tolerance: float) -> List[RouteCandidateInfo]:
        pts: List[RouteCandidateInfo] = []
        for info in self.__locator.get(pt, tolerance):
            pts.append(info)

        return pts

    def __add_route(self, rt: PointXyzList) -> None:
        if (len(rt) < 10):
            return

        i = 0
        for p in rt:
            self.__locator.put(RouteCandidateInfo(p, i, len(self.__routes)))
            i += 1

        self.__routes.append(rt.copy())
