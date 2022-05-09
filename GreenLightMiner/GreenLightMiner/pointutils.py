import math

from pointgps import PointGps, GpsRoute
from direction2d import Direction2d
from pointxyz import PointXyz, XyzRoute

__EARTH_RADIUS_METERS = 6378100.0


def __gps_point_to_xyz_point(gps_point: PointGps) -> PointXyz:
    latRad = (gps_point.lat * math.pi) / 180.0
    lonRad = (gps_point.lng * math.pi) / 180.0

    cosLat = math.cos(latRad)

    return PointXyz(
        __EARTH_RADIUS_METERS * cosLat * math.cos(lonRad),
        __EARTH_RADIUS_METERS * cosLat * math.sin(lonRad),
        0
        # __EARTH_RADIUS_METERS * math.sin(latRad) + gps_point.alt
    )


def gps_route_to_xyz_route(gps_route: GpsRoute) -> XyzRoute:
    return [__gps_point_to_xyz_point(p) for p in gps_route]


def distance(p1: PointXyz, p2: PointXyz) -> float:
    return math.sqrt(math.pow(p1.x - p2.x, 2) + math.pow(p1.y - p2.y, 2) + math.pow(p1.z - p2.z, 2))


def get_direction2d(p_from: PointXyz, p_to: PointXyz) -> Direction2d:
    return Direction2d(p_to.x - p_from.x, p_to.y - p_from.y)


def get_angle_between(d1: Direction2d, d2: Direction2d) -> float:
    if d1 is None or d2 is None:
        raise Exception("get_angle_between parameter is None")

    if not (d1.is_valid() and d2.is_valid()):
        raise Exception("get_angle_between parameter is invalid")

    a = (d1.x * d2.x + d1.y * d2.y) / (math.sqrt(d1.x * d1.x + d1.y * d1.y) * (math.sqrt(d2.x * d2.x + d2.y * d2.y)))
    ac = math.acos(a)

    return math.degrees(ac)


def check_same_direction(d1: Direction2d, d2: Direction2d, angle: float) -> bool:
    return get_angle_between(d1, d2) <= angle
