curl -X POST \
  http://keycloak:8080/realms/Test/protocol/openid-connect/token \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'grant_type=password&client_id=test-client&client_secret=mA1VxFslWGukos6JquOZcoU7qVUElsmv&username=test&password=test'
