protoc -I ./Protos/ ./Protos/greet.proto \
    --js_out=import_style=commonjs:../NuxtSample/src/middleware \
    --grpc-web_out=import_style=commonjs+dts,mode=grpcwebtext:../NuxtSample/src/middleware

protoc \
    --plugin="protoc-gen-ts=./src/middleware" \
    --plugin=protoc-gen-grpc=./src/middleware \
    --js_out="import_style=commonjs,binary:./src/middleware" \
    --ts_out="service=grpc-node:./src/middleware" \
    --grpc_out="./src/middleware" \
    ../AspNetCoreSample.Grpc/Protos/*.proto
