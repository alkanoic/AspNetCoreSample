curl -k -o swagger.json https://127.0.0.1:7035/swagger/v1/swagger.json

npx quicktype --src-lang json --lang ts swagger.json -o webapi.ts
