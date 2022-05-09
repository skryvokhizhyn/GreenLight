import sqlite3
from typing import Dict, Any

from pointgps import GpsRoutes, PointGps, GpsRoute

class SourceDb:
    def __init__(self, db_pas: str):
        self.__conn = sqlite3.connect(db_pas)

    def load(self) -> GpsRoutes:
        cur = self.__conn.cursor()
        cur.execute("SELECT timestamp, longitude, latitude, altitude, speed, ride_id FROM GpsLocation")

        return self.__get_routes(cur.fetchall())

    def __get_routes(self, rows: Any) -> GpsRoutes:
        routes: Dict[str, GpsRoute] = {}

        for row in rows:
            if len(row) > 0:
                routes.setdefault(row[5], []).append(
                    PointGps(row[0], row[1], row[2], row[3]))

        return [rt for rt in routes.values()]
