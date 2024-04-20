grpcurl -d '{ \"name\": \"World\" }' localhost:7189 greet.Greeter/SayHello

grpcurl -plaintext -d '{ "name": "World" }' localhost:5167 greet.Greeter/SayHello
grpcurl -plaintext localhost:5167 list
grpcurl -plaintext localhost:5167 describe
