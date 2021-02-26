import sys

import gpsutils
import route
import routeutils
import source


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

    for r in routes.values():
        print(len(routeutils.remove_close_points(
            gpsutils.gps_route_to_xyz_route(r), 10)))


if __name__ == "__main__":
    try:
        main()
    except Exception as ex:
        print("Error: " + str(ex))
