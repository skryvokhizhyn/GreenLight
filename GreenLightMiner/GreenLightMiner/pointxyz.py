from typing import NamedTuple, List


class PointXyz(NamedTuple):
    x: float
    y: float
    z: float


XyzRoute = List[PointXyz]
XyzRoutes = List[XyzRoute]
