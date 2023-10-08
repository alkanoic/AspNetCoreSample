curl -k -o swagger.json https://127.0.0.1:7035/swagger/v1/swagger.json

# npx openapi-typescript https://localhost:7035/api/v1/swagger.json -o ./schema.d.ts
npx openapi-typescript ./swagger.json -o ./schema.d.ts
