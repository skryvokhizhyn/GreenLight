from aws_cdk import core
from aws_cdk import aws_ec2 as ec2
from aws_cdk import aws_lambda as lamb
from aws_cdk import aws_apigateway as agw
from aws_cdk import aws_dynamodb

class GreenLightBubaStack(core.Stack):

    def __init__(self, scope: core.Construct, construct_id: str, **kwargs) -> None:
        super().__init__(scope, construct_id, **kwargs)

        vpc = ec2.Vpc(self, id="BubaVpc", cidr="192.168.0.0/24", max_azs=1,
            subnet_configuration=[ec2.SubnetConfiguration(cidr_mask=26, name="Isolated", subnet_type=ec2.SubnetType.ISOLATED)])

        la = lamb.Function(self, id="RawRotePOSTLambda", runtime=lamb.Runtime.PYTHON_3_6, code=lamb.Code.from_asset("lambda"), handler="raw_rote_post.handler")

        api = agw.RestApi(self, id="AGW")

        collectResource = api.root.add_resource("collect")
        collectResource.add_method("POST", agw.LambdaIntegration(la))

        table = aws_dynamodb.Table(self, "RawRoteV1", 
            partition_key=aws_dynamodb.Attribute(name="route_id", type=aws_dynamodb.AttributeType.STRING),
            sort_key=aws_dynamodb.Attribute(name="ts", type=aws_dynamodb.AttributeType.STRING)
            )

        la.add_environment("RAW_ROUTE_TABLE", table.table_name)

        table.grant_write_data(la)