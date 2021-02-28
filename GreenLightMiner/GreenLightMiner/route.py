from typing import List


class RouteGpsPoint:
    def __init__(self, ts: int, lng: float, lat: float, alt: float):
        self.__ts = ts
        self.__lng = lng
        self.__lat = lat
        self.__alt = alt

    @property
    def lng(self) -> float:
        return self.__lng

    @property
    def lat(self) -> float:
        return self.__lat

    @property
    def alt(self) -> float:
        return self.__alt

    @property
    def ts(self) -> int:
        return self.__ts


class RouteXyzPoint:
    def __init__(self, ts: int, x: float, y: float, z: float):
        self.__ts = ts
        self.__x = x
        self.__y = y
        self.__z = z

    def __eq__(self, other):
        return self.__ts == other.__ts and self.__x == other.__x and self.__y == other.__y and self.__z == other.__z

    @property
    def x(self) -> float:
        return self.__x

    @property
    def y(self) -> float:
        return self.__y

    @property
    def z(self) -> float:
        return self.__z

    @property
    def ts(self) -> float:
        return self.__ts


GpsRoute = List[RouteGpsPoint]
XyzRoute = List[RouteXyzPoint]
