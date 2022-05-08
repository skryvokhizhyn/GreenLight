import json
from typing import Dict

import boto3

from pointgps import GpsRoutes, PointGps, GpsRoute

class SourceAws:
    def load(self) -> GpsRoutes:
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
            res = {**res, **self.__parse_aws_items(response.get('Items', []))}

            start_key = response.get('LastEvaluatedKey', None)
            done = start_key is None

        return [rt for rt in res.values()]

    def __parse_aws_items(self, items) -> Dict[str, GpsRoute]:
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
