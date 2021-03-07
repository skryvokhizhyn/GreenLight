from typing import List

import pointutils
import route
import routeutils
from xyminmax import XYMinMax


def get_mid_point_index(rt: route.XyzRoute, indexFrom: int, indexTo: int) -> int:
    if indexFrom < 0 or indexFrom < 0 or indexTo >= len(rt) or indexFrom > indexTo:
        raise Exception("Wrong indices given to get_mid_point_index: " + str(len(rt)) + " " + str(indexFrom) + " " + str(indexTo))

    if indexTo - indexFrom < 2:
        return None

    if indexTo - indexFrom == 2:
        return int((indexFrom + indexTo) / 2)

    m = None
    i = indexFrom + 1
    minDiff = 1000000

    pFrom = rt[indexFrom]
    pTo = rt[indexTo]

    while i < indexTo:
        pI = rt[i]
        diff = abs(pointutils.distance(pFrom, pI) - pointutils.distance(pI, pTo))
        if minDiff > diff:
            m = i
            minDiff = diff

        i += 1

    return int(m)


def remove_all_none(rt: route.XyzRoute) -> None:
    ln = len(rt)

    k = 0
    while k < ln and rt[k] is None:
        k += 1

    if k == ln:
        rt.clear()
        return

    first_non_None = k
    k += 1

    while k < ln and not rt[k] is None:
        k += 1

    i = k + 1

    while i < ln:
        if not rt[i] is None:
            rt[k] = rt[i]
            k += 1

        i += 1

    if k > first_non_None + 1:
        del rt[k:]

    if first_non_None > 0:
        del rt[:first_non_None]


def remove_close_points(rt: route.XyzRoute, dist_m: float) -> None:
    if len(rt) <= 2:
        return rt

    j = 0
    i = 1

    while i < len(rt):
        if pointutils.distance(rt[j], rt[i]) < dist_m:
            i += 1
            continue

        m = None
        if i - j > 2:
            m = routeutils.get_mid_point_index(rt, j, i)

        for k in range(j + 1, i - 1):
            if k != m:
                rt[k] = None

        j = i
        i += 1

    for k in range(j + 1, i - 1):
        rt[k] = None

    remove_all_none(rt)


def get_length(rt: route.XyzRoute) -> float:
    if len(rt) < 2:
        return 0

    sm = 0

    for i in range(1, len(rt)):
        sm += pointutils.distance(rt[i-1], rt[i])

    return sm


def split_by_time(rt: route.XyzRoute, split_ms: int) -> List[route.XyzRoute]:
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


def get_routes_xy_min_max(rts: List[route.XyzRoute]) -> XYMinMax:
    min_max = XYMinMax(100000000, 1000000000, -1000000000, -1000000000)

    for rt in rts:
        for p in rt:
            min_max = XYMinMax(x_min=min(min_max.x_min, p.x), y_min=min(min_max.y_min, p.y), x_max=max(min_max.x_max, p.x), y_max=max(min_max.y_max, p.y))

    return min_max
