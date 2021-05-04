import sys
import traceback
from typing import Dict, List

import pointutils
import pointxyz
import route
import routeutils
import source
from gui.routesvisualizer import RoutesVisualizer
from pointxyz import PointXyzList
from routeaggregator import RouteAggregator
from routeutils import split_by_time


def main() -> None:
    if len(sys.argv) < 3:
        raise Exception("Specify source type. E.g.: '-t db' or '-t aws'")

    if sys.argv[1] != "-t":
        raise Exception("The only expected parameter is '-t'")

    routes: Dict[str, route.GpsRoute] = {}

    if sys.argv[2] == "db":
        if (len(sys.argv) < 4):
            raise Exception("no path to DB specified")

        routes = source.get_routes_from_db(sys.argv[3])
    elif sys.argv[2] == "aws":
        routes = source.get_routes_from_aws()
    else:
        raise Exception("Unexpected source type. Valid options are [db, aws]")

    gps_routes: List[route.GpsRoute] = []
    for rt in list(routes.values()):
        for r in routeutils.split_by_time(rt, 5000):
            gps_routes.append(r)

    xyz_routes: List[PointXyzList] = []
    for rt in gps_routes:
        xyz_routes.append(pointutils.gps_route_to_xyz_route(rt))

    preprocessed_routes: List[PointXyzList] = []
    for rr in xyz_routes:
        initial_len = len(rr)
        routeutils.remove_close_points(rr, 10)

        if routeutils.get_length(rr) < 100:
            print("route skipped")
            continue

        print(str(initial_len) + " " + str(len(rr)) + " " + str(routeutils.get_length(rr)))

        preprocessed_routes.append(rr)

    preprocessed_routes.sort(key=lambda r: len(r), reverse=True)

    xy_min_max = routeutils.get_routes_xy_min_max(xyz_routes)

    viewVisualizer = RoutesVisualizer(xy_min_max)
    aggregator = RouteAggregator(tolerance_dist=10, tolerance_angle=15)

    for r1 in preprocessed_routes:
        aggregator.consume_route(r1)

    res = aggregator.extend_routes()

    for r2 in aggregator.routes:
        viewVisualizer.add_points(r2)

    viewVisualizer.run()


if __name__ == "__main__":
    try:
        main()
    except Exception:
        traceback.print_exc()
