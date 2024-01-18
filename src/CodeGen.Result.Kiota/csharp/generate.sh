# 事前に起動していること
# dotnet run --project ../../CodeGen.Result
dotnet kiota generate -l csharp --openapi http://localhost:5100/swagger/v1/swagger.json -c ApiClient -o ./ApiClient

# curl -o swagger.json http://localhost:5100/swagger/v1/swagger.json
# dotnet kiota generate -l csharp --openapi ./swagger.json -c ApiClient -o ./ApiClient
