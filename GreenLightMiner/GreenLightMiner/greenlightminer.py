import sys
import traceback
from typing import List

import pointutils
import route
import routeutils
import source
from gui.routesvisualizer import RoutesVisualizer
from pointxyz import PointXyzList
from routeaggregator import RouteAggregator


def main() -> None:
    if len(sys.argv) < 3:
        raise Exception("Specify source type. E.g.: '-t db' or '-t aws'")

    if sys.argv[1] != "-t":
        raise Exception("The only expected parameter is '-t'")

    routes = {}

    if sys.argv[2] == "db":
        if (len(sys.argv) < 4):
            raise Exception("no path to DB specified")

        routes = source.get_routes_from_db(sys.argv[3])
    elif sys.argv[2] == "aws":
        routes = source.get_routes_from_aws()
    else:
        raise Exception("Unexpected source type. Valid options are [db, aws]")

    gps_routes: route.GpsRoute = []
    tmp = [gps_routes.extend(routeutils.split_by_time(rt, 5000)) for rt in routes.values()]

    xyz_routes: PointXyzList = []
    tmp = [xyz_routes.append(pointutils.gps_route_to_xyz_route(rt)) for rt in gps_routes]

    tmp.clear()

    xy_min_max = routeutils.get_routes_xy_min_max(xyz_routes)

    preprocessed_routes: List[PointXyzList] = []

    for rt in xyz_routes:
        initial_len = len(rt)

        routeutils.remove_close_points(rt, 10)

        if routeutils.get_length(rt) < 100:
            print("route skipped")
            continue

        print(str(initial_len) + " " + str(len(rt)) + " " + str(routeutils.get_length(rt)))

        preprocessed_routes.append(rt)

    preprocessed_routes.sort(key=lambda r: len(r), reverse=True)

    viewVisualizer = RoutesVisualizer(xy_min_max)
    aggregator = RouteAggregator(tolerance_dist=10, tolerance_angle=15)

    for r in preprocessed_routes:
        aggregator.consume_route(r)

    for r in aggregator.routes:
        viewVisualizer.add_points(r)

    viewVisualizer.run()


if __name__ == "__main__":
    try:
        main()
    except Exception:
        traceback.print_exc()
