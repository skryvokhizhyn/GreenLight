from typing import List

class RoutePoint:
    def __init__(self, ts: int, lng: float, lat: float, alt: float):
        self.__ts = ts
        self.__lng = lng
        self.__lat = lat
        self.__alt = alt


Route = List[RoutePoint]