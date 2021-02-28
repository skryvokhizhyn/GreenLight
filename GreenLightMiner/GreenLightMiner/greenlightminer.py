import sys

import pointutils
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
        rt = pointutils.gps_route_to_xyz_route(r)
        routeutils.remove_close_points(rt, 10)

        if routeutils.get_length(rt) < 100:
            print("route skipped")
            continue

        print(str(len(r)) + " " + str(len(rt)) + " " + str(routeutils.get_length(rt)))


if __name__ == "__main__":
    try:
        main()
    except Exception as ex:
        print("Error: " + str(ex))
