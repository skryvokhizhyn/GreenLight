import math
from typing import Dict, NamedTuple, Set, List

from pointxy import PointXy


class _Size2d(NamedTuple):
    x: int
    y: int


class GridMap2d:
    def __init__(self, cell_size: float):
        self.__cell_size = cell_size
        self.__grid: Dict[_Size2d, list] = {}

    def put(self, p: PointXy, item) -> None:
        pos = self.__get_pos(p)
        self.__grid.setdefault(pos, []).append(item)

    def get(self, p: PointXy, dist: float) -> list:
        poss = self.__get_pos_within_dist(p, dist)

        res: list = []

        for pos in poss:
            items = self.__grid.get(pos)
            if items is not None and len(items) > 0:
                res += items

        return res

    def __get_pos(self, p: PointXy) -> _Size2d:
        return _Size2d(math.floor(p.x / self.__cell_size), math.floor(p.y / self.__cell_size))

    def __get_pos_within_dist(self, p: PointXy, dist: float) -> List[_Size2d]:
        poss: Set[_Size2d] = set()

        x_min: int = math.floor((p.x - dist) / self.__cell_size)
        x_max: int = math.floor((p.x + dist) / self.__cell_size)
        y_min: int = math.floor((p.y - dist) / self.__cell_size)
        y_max: int = math.floor((p.y + dist) / self.__cell_size)

        for i in range(x_min, x_max + 1):
            for j in range(y_min, y_max + 1):
                poss.add(_Size2d(i, j))

        return [s for s in poss]
