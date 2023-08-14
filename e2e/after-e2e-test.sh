#!/bin/bash

pid=$(ps aux | grep 'dotnet run --project ../src/AspNetCoreSample.Mvc' | grep -v grep | awk '{print $2}')
if [ -n "$pid" ]; then
    kill $pid
fi
