
from multiprocessing import connection
import utils
from typing import Dict, Set, Tuple, List
from pointxyz import PointXyz, XyzRoute, XyzRoutes
from routecancidateinfolocator import RouteCandidateInfoLocator
from routecandidateinfo import RouteCandidateInfo
import pointutils


class RouteExtender:
    def __init__(self, tolerance_dist: float):
        self.__tolerance: float = tolerance_dist
        self.__locator_starts = RouteCandidateInfoLocator(self.__tolerance * 10)
        self.__locator_ends = RouteCandidateInfoLocator(self.__tolerance * 10)

    def extend(self, routes: XyzRoutes) -> XyzRoutes:
        route_id: int = 0

        connections: List[Tuple[int, int]] = []

        for route in routes:
            start_point: PointXyz = route[0]

            nearest_to_start_point = self.__locator_ends.get(start_point, self.__tolerance)
            nearest_to_start_point.sort(key = lambda p: pointutils.distance(p.point, start_point))

            if len(nearest_to_start_point) > 0:
                connections.append((nearest_to_start_point[0].route_id, route_id))
            else:
                self.__locator_starts.put(RouteCandidateInfo(route[0], 0, route_id))

            end_point: PointXyz = route[-1]

            nearest_to_end_point = self.__locator_starts.get(end_point, self.__tolerance)
            nearest_to_end_point.sort(key = lambda p: pointutils.distance(p.point, end_point))

            if len(nearest_to_end_point) > 0:
                connections.append((route_id, nearest_to_end_point[0].route_id))
            else:
                self.__locator_ends.put(RouteCandidateInfo(end_point, len(route) - 1, route_id))

            route_id += 1

        connected_ranges = utils.connect_ranges(connections)

        result: XyzRoutes = []

        processed_routes: Set[int] = set()

        for (start, chain) in connected_ranges:
            extended_route: XyzRoute = list(routes[start])

            processed_routes.add(start)

            for extendee in chain:
                extended_route.extend(routes[extendee])
                processed_routes.add(extendee)

            result.append(extended_route)

        for i in range(0, route_id):
            if i not in processed_routes:
               result.append(routes[i])

        return result