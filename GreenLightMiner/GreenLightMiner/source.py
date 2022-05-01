import json
import sqlite3
from typing import Dict, List, Any

import boto3

from pointgps import PointGps, GpsRoute


def __get_routes(rows: Any) -> Dict[str, GpsRoute]:
    routes: Dict[str, GpsRoute] = {}

    for row in rows:
        if len(row) > 0:
            routes.setdefault(row[5], []).append(
                PointGps(row[0], row[1], row[2], row[3]))

    return routes


def get_routes_from_db(db_path: str) -> Dict[str, GpsRoute]:
    conn = sqlite3.connect(db_path)

    cur = conn.cursor()
    cur.execute(
        "SELECT timestamp, longitude, latitude, altitude, speed, ride_id FROM GpsLocation")

    return __get_routes(cur.fetchall())


def __parse_aws_items(items) -> Dict[str, GpsRoute]:
    routes: Dict[str, GpsRoute] = {}

    for item in items:
        payload = json.loads(item['payload'])

        if not payload['type'] == 'string':
            raise Exception("Unsupported payload type '" +
                            payload['type'] + "'. Expected 'string' only")

        route_id = item['route_id']

        data = payload['data']

        for d in json.loads(data):
            routes.setdefault(route_id, []).append(PointGps(
                d['Longitude'], d['Latitude'], d['Altitude'], d['Speed']))

    return routes


def get_routes_from_aws() -> Dict[str, GpsRoute]:

    client = boto3.client("dynamodb")
    table_names = client.list_tables(Limit=20)

    raw_route_table_name = None

    expected_table_name = 'RawRoteV1'

    for t in table_names['TableNames']:
        if t.find(expected_table_name) != -1:
            raw_route_table_name = t
            break

    if raw_route_table_name is None:
        raise Exception("DynamoDB table " + expected_table_name + " not found")

    db = boto3.resource("dynamodb")
    table = db.Table(raw_route_table_name)

    scan_kwargs = {
        'ProjectionExpression': "route_id, payload"
    }

    done = False
    start_key = None

    res: Dict[str, GpsRoute] = {}

    while not done:
        if start_key:
            scan_kwargs['ExclusiveStartKey'] = start_key

        response = table.scan(**scan_kwargs)
        res = {**res, **__parse_aws_items(response.get('Items', []))}

        start_key = response.get('LastEvaluatedKey', None)
        done = start_key is None

    return res
