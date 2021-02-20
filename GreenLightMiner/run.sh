#!/bin/bash

if [[ $1 = "db" ]]; then
    eval "AWS_PROFILE=default python GreenLightMiner/greenlightminer.py -t db testdata/db_green_light_tracker.db"
elif [[ $1 = "aws" ]]; then
    eval "AWS_PROFILE=default python GreenLightMiner/greenlightminer.py -t aws"
fi
