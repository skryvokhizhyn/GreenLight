from typing import List

import pointutils
from gridmap2d import GridMap2d
from pointxy import PointXy
from route import RouteXyzPoint
from routecandidateinfo import RouteCandidateInfo


class RouteCandidateInfoLocator:
    def __init__(self, cell_size: float):
        self.__grid = GridMap2d(cell_size)

    def put(self, info: RouteCandidateInfo) -> None:
        self.__grid.put(PointXy(info.point.x, info.point.y), info)

    def get(self, p: RouteXyzPoint, dist: float) -> List[RouteCandidateInfo]:
        res = self.__grid.get(PointXy(p.x, p.y), dist)

        return [i for i in res if pointutils.distance(i.point, p) <= dist]
