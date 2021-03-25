import sys
import traceback

import pointutils
import routeutils
import source

from routeaggregator import RouteAggregator
from gui.routesvisualizer import RoutesVisualizer


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

    xyz_routes = [pointutils.gps_route_to_xyz_route(r) for r in routes.values()]

    xy_min_max = routeutils.get_routes_xy_min_max(xyz_routes)

    viewVisualizer = RoutesVisualizer(xy_min_max)
    aggregator = RouteAggregator(tolerance_dist=10, tolerance_angle=15)

    for rt in xyz_routes:
        routeutils.remove_close_points(rt, 10)

        split_routes = routeutils.split_by_time(rt, 5000)

        for sub_route in split_routes:
            if routeutils.get_length(sub_route) < 100:
                print("route skipped")
                continue

            print(str(len(rt)) + " " + str(len(sub_route)) + " " + str(routeutils.get_length(sub_route)))

            # viewVisualizer.add_points(sub_route)
            aggregator.consume_route(sub_route)

    for r in aggregator.routes:
        viewVisualizer.add_points(r)

    viewVisualizer.run()


if __name__ == "__main__":
    try:
        main()
    except Exception:
        traceback.print_exc()
