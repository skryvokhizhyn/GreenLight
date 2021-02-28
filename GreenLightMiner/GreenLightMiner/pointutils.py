import math

from route import GpsRoute, RouteGpsPoint, RouteXyzPoint, XyzRoute

__EARTH_RADIUS_METERS = 6378100.0


def __gps_point_to_xyz_point(gps_point: RouteGpsPoint) -> RouteXyzPoint:
    latRad = (gps_point.lat * math.pi) / 180.0
    lonRad = (gps_point.lng * math.pi) / 180.0

    cosLat = math.cos(latRad)

    return RouteXyzPoint(gps_point.ts, __EARTH_RADIUS_METERS * cosLat * math.cos(lonRad), __EARTH_RADIUS_METERS * cosLat * math.sin(lonRad), __EARTH_RADIUS_METERS * math.sin(latRad) + gps_point.alt)


def gps_route_to_xyz_route(gps_route: GpsRoute) -> XyzRoute:
    return [__gps_point_to_xyz_point(p) for p in gps_route]


def distance(p1: RouteXyzPoint, p2: RouteXyzPoint) -> float:
    return math.sqrt(math.pow(p1.x - p2.x, 2) + math.pow(p1.y - p2.y, 2) + math.pow(p1.z - p2.z, 2))
