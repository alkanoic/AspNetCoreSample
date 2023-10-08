curl -k -o swagger.json https://127.0.0.1:7035/swagger/v1/swagger.json

npx openapi2aspida -i ./swagger.json
