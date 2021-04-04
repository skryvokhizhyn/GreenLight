from typing import List, Optional

from routecandidateinfo import RouteCandidateInfo

import pointutils
import route
import routeutils
import utils
from xyminmax import XYMinMax
from pointxyz import PointXyz, PointXyzList


def get_mid_point_index(rt: PointXyzList, indexFrom: int, indexTo: int) -> Optional[int]:
    if indexFrom < 0 or indexFrom < 0 or indexTo >= len(rt) or indexFrom > indexTo:
        raise Exception("Wrong indices given to get_mid_point_index: " + str(len(rt)) + " " + str(indexFrom) + " " + str(indexTo))

    if indexTo - indexFrom < 2:
        return None

    if indexTo - indexFrom == 2:
        return int((indexFrom + indexTo) / 2)

    m = None
    i = indexFrom + 1
    minDiff: float = 1000000

    pFrom = rt[indexFrom]
    pTo = rt[indexTo]

    while i < indexTo:
        pI = rt[i]
        diff = abs(pointutils.distance(pFrom, pI) - pointutils.distance(pI, pTo))
        if minDiff > diff:
            m = i
            minDiff = diff

        i += 1

    return m


def remove_close_points(rt: PointXyzList, dist_m: float) -> None:
    if len(rt) <= 2:
        return

    j = 0
    i = 1

    while i < len(rt):
        if pointutils.distance(rt[j], rt[i]) < dist_m:
            i += 1
            continue

        m = None
        if i - j > 2:
            m = routeutils.get_mid_point_index(rt, j, i)

            for k in range(j + 1, i):
                if k != m:
                    rt[k] = None  # type: ignore

        j = i
        i += 1

    for k in range(j + 1, i - 1):
        rt[k] = None  # type: ignore

    utils.remove_all_none_from_list(rt)


def get_length(rt: PointXyzList) -> float:
    if len(rt) < 2:
        return 0

    sm: float = 0

    for i in range(1, len(rt)):
        sm += pointutils.distance(rt[i - 1], rt[i])

    return sm


def split_by_time(rt: route.GpsRoute, split_ms: int) -> List[route.GpsRoute]:
    if len(rt) < 2:
        return [rt]

    res = []
    j = 0
    i = 1

    while i < len(rt):
        if rt[i].ts - rt[i - 1].ts > split_ms:
            res.append(rt[j:i])
            j = i

        i += 1

    res.append(rt[j:i])

    return res


def get_routes_xy_min_max(rts: List[PointXyzList]) -> XYMinMax:
    min_max = XYMinMax(100000000, 1000000000, -1000000000, -1000000000)

    for rt in rts:
        for p in rt:
            min_max = XYMinMax(x_min=min(min_max.x_min, p.x), y_min=min(min_max.y_min, p.y), x_max=max(min_max.x_max, p.x), y_max=max(min_max.y_max, p.y))

    return min_max


def advance_candidates(point: PointXyz, candidates: List[RouteCandidateInfo], routes: List[PointXyzList], tolerance: float) -> None:
    for candidate_id, candidate in enumerate(candidates):
        if candidate.route_id >= len(routes):
            raise Exception('Candidate route id is out of range')

        rt = routes[candidate.route_id]

        if candidate.point_id >= len(rt):
            raise Exception('Candidate point is out of range')

        pt_id = candidate.point_id
        checked_point: Optional[PointXyz] = None
        checked_point_id: int = 0

        while pt_id < len(rt) and pointutils.distance(point, rt[pt_id]) <= tolerance:
            checked_point = rt[pt_id]
            checked_point_id = pt_id
            pt_id += 1

        if checked_point is None:
            candidates[candidate_id] = None  # type: ignore
        else:
            candidates[candidate_id] = RouteCandidateInfo(checked_point, checked_point_id, candidate.route_id)

    utils.remove_all_none_from_list(candidates)


def remove_not_same_direction(point_prev: PointXyz, point_curr: PointXyz, candidates: List[RouteCandidateInfo], routes: List[PointXyzList], tolerance_degrees: float) -> None:
    for candidate_id, candidate in enumerate(candidates):
        if candidate.route_id >= len(routes):
            raise Exception('Candidate route id is out of range')

        rt = routes[candidate.route_id]

        if candidate.point_id >= len(rt):
            raise Exception('Candidate point is out of range')

        # treat candidates with id 0 as always matching
        if candidate.point_id == 0:
            continue

        candidate_point_prev = rt[candidate.point_id - 1]
        candidate_point_curr = rt[candidate.point_id]

        point_dir = pointutils.get_direction2d(point_prev, point_curr)
        point_candidate_dir = pointutils.get_direction2d(candidate_point_prev, candidate_point_curr)

        if pointutils.get_angle_between(point_dir, point_candidate_dir) > tolerance_degrees:
            candidates[candidate_id] = None  # type: ignore

    utils.remove_all_none_from_list(candidates)
