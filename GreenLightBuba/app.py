#!/usr/bin/env python3

from aws_cdk import core

from green_light_buba.green_light_buba_stack import GreenLightBubaStack


app = core.App()
GreenLightBubaStack(app, "green-light-buba")

app.synth()
