from typing import List

import pointutils
import route
import routeutils
from routecandidateinfo import RouteCandidateInfo


class RouteAggregator:
    def __init__(self):
        self.__tolerance = 1
        self.__codirection_angle_degrees = 10
        self.__routes: List[route.XyzRoute] = []
        self.__infos: List[RouteCandidateInfo] = []

    # doesn't aggregate co-directional routes and ignores points
    def consume_route(self, rt: route.XyzRoute) -> None:
        candidates: List[RouteCandidateInfo] = []
        buffered_routes: List[route.XyzRoute] = []

        prev_filtered: int = -1

        for i in range(len(rt)):
            routeutils.advance_candidates(rt[i], candidates, self.__routes, self.__tolerance)

            if len(candidates) == 0:
                candidates = self.__get_closest_points(rt[i])

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

    @property
    def routes(self) -> List[route.XyzRoute]:
        return self.__routes

    def __get_closest_points(self, pt: route.RouteXyzPoint) -> List[RouteCandidateInfo]:
        pts: List[RouteCandidateInfo] = []
        for info in self.__infos:
            if pointutils.distance(pt, info.point) <= self.__tolerance:
                pts.append(info)

        return pts

    def __add_route(self, rt: route.XyzRoute) -> None:
        if (len(rt) < 2):
            return

        i = 0
        for p in rt:
            self.__infos.append(RouteCandidateInfo(p, len(self.__routes), i))

        self.__routes.append(rt.copy())
