from pointxyz import XyzRoute, XyzRoutes
from routecancidateinfolocator import RouteCandidateInfoLocator
from routecandidateinfo import RouteCandidateInfo

class RouteCrossroadSplitter:
    def __init__(self, tolerance_dist: float, equality_dist: float):
        self.__tolerance: float = tolerance_dist
        self.__equality_dist: float = equality_dist
        self.__route_edges_locator = RouteCandidateInfoLocator(self.__tolerance * 10)
        self.__result: XyzRoutes = []

    def split(self, routes: XyzRoutes) -> XyzRoutes:
        for route_id, route in enumerate(routes):
            self.__route_edges_locator.put(RouteCandidateInfo(route[0], 0, route_id))
            self.__route_edges_locator.put(RouteCandidateInfo(route[-1], len(route) - 1, route_id))

        for route_id, route in enumerate(routes):
            self.__process_route(route_id, route)

        return self.__result

    def __process_route(self, route_id: int, route: XyzRoute) -> None:

        route_start = 0

        for id, pt in enumerate(route):
            candidates = [c for c in self.__route_edges_locator.get(pt, self.__equality_dist) if c.route_id != route_id]

            if len(candidates) > 0:
                self.__add_route(route[route_start:id + 1])
                route_start = id

        if route_start < id:
            self.__add_route(route[route_start:id + 1])

    def __add_route(self, route: XyzRoute) -> None:
        self.__result.append(route)
