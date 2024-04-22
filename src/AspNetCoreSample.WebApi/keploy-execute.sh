keploy record -c "dotnet run"

curl -X 'POST' -k \
  'https://localhost:7035/Auth' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "userName": "admin",
  "password": "admin"
}'

curl -X 'GET' -k \
  'https://localhost:7035/Simple?Input=123' \
  -H 'accept: application/json'

curl -X 'POST' -k \
  'https://localhost:7035/Simple' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "input": "string"
}'

curl -X 'GET' -k \
  'https://localhost:7035/Name' \
  -H 'accept: application/json'

curl -X 'POST' -k \
  'https://localhost:7035/Name' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": 0,
  "name1": "string"
}'

curl -X 'DELETE' -k \
  'https://localhost:7035/Name?id=4' \
  -H 'accept: */*'
