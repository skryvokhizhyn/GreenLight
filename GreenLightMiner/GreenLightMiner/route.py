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


GpsRoute = List[RouteGpsPoint]
