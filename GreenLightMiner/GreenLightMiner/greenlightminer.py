import sys
import traceback

import pointutils
from pointgps import GpsRoutes
from routecrossroadsplitter import RouteCrossroadSplitter
from routeextender import RouteExtender
import routeutils
import source
from gui.routesvisualizer import RoutesVisualizer
from pointxyz import XyzRoutes
from routeaggregator import RouteAggregator


def main() -> None:
    if len(sys.argv) < 3:
        raise Exception("Specify source type. E.g.: '-t db' or '-t aws'")

    if sys.argv[1] != "-t":
        raise Exception("The only expected parameter is '-t'")

    loader = source.get_source(sys.argv)
    routes: GpsRoutes = loader.load()

    gps_routes: GpsRoutes = []
    for rt in list(routes):
        for r in routeutils.split_by_time(rt, 5000):
            gps_routes.append(r)

    xyz_routes: XyzRoutes = []
    for rt in gps_routes:
        xyz_routes.append(pointutils.gps_route_to_xyz_route(rt))

    preprocessed_routes: XyzRoutes = []
    for rr in xyz_routes:
        routeutils.remove_close_points(rr, 5)

    preprocessed_routes = [r for r in xyz_routes if routeutils.get_length(r) >= 500]
    preprocessed_routes.sort(key=lambda r: len(r), reverse=True)
    #preprocessed_routes = map(lambda r: routeutils.enrich_with_mid_points(r, 5), preprocessed_routes)

    xy_min_max = routeutils.get_routes_xy_min_max(xyz_routes)

    #aggregated_routes: XyzRoutes = RouteAggregator(tolerance_dist=20, tolerance_angle=15).aggregate_routes(preprocessed_routes)
    #extended_routes: XyzRoutes = RouteExtender(tolerance_dist=10).extend(aggregated_routes)
    #split_routes: XyzRoutes = RouteCrossroadSplitter(tolerance_dist=20, equality_dist=10).split(extended_routes)

    viewVisualizer = RoutesVisualizer(xy_min_max)

    #for r2 in split_routes:
    for r2 in preprocessed_routes:
        viewVisualizer.add_points(r2)

    viewVisualizer.run()


if __name__ == "__main__":
    try:
        main()
    except Exception:
        traceback.print_exc()
