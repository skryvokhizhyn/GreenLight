import json
import boto3
import os
from botocore.exceptions import ClientError

dynamodb = boto3.resource("dynamodb")
tableName = os.environ['RAW_ROUTE_TABLE']

def handler(event, context):
    table = dynamodb.Table(tableName)
   
    body = json.loads(event['body'])
    
    try:
        route_id = body['route_id']
        ts = body['ts']
        val = body['payload']

        table.put_item(Item={'route_id': route_id, "ts": ts, "payload": val})

    except ClientError as ex:
        return {
            'statusCode': 500,
            'headers': {'Content-Type': 'text/plain'},
            'body': ex.response
        }
    except Exception as ex:
        return {
            'statusCode': 500,
            'headers': {'Content-Type': 'text/plain'},
            'body': ex
        }

    return {
        'statusCode': 200
    }
