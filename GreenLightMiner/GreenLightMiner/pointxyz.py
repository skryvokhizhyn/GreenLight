from typing import NamedTuple, List


class PointXyz(NamedTuple):
    x: float
    y: float
    z: float


PointXyzList = List[PointXyz]
