#!/bin/bash

docker compose down -v
docker compose up -d
rm -r -f allure-results
bash after-e2e-test.sh
dotnet run --project ../src/AspNetCoreSample.Mvc &
for i in $(seq 1 10); do
    sleep 2;
    healty=$(docker inspect --format='{{.State.Health.Status}}' mysql)
    if [ $healty = "healthy" ]; then
        break;
    else
        echo -n .
    fi
done
