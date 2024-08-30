#!/bin/bash -x

aws s3 cp s3://data-service-config/test.txt /home --profile staging
dotnet xe.data.service.dll