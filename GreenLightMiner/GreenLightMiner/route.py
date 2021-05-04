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

    def __eq__(self, other: object) -> bool:
        if not isinstance(other, RouteGpsPoint):
            return NotImplemented

        return self.__ts == other.__ts and self.__lng == other.__lng and self.__lat == other.__lat and self.__alt == other.__alt


GpsRoute = List[RouteGpsPoint]
