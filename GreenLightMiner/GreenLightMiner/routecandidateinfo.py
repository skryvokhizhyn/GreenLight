from typing import NamedTuple, List
from pointxyz import PointXyz


class RouteCandidateInfo(NamedTuple):
    point: PointXyz
    point_id: int
    route_id: int

RouteCandidateInfos = List[RouteCandidateInfo]