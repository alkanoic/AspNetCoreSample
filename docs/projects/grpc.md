# gRPC サービス

`AspNetCoreSample.Grpc` は gRPC サーバーアプリケーションです。

## 起動

```bash
dotnet run --project src/AspNetCoreSample.Grpc
```

## サービス定義

### GreeterService

`Protos/greet.proto` で定義された標準的な Hello World gRPC サービスです。

```protobuf
service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
```

## 機能

- **gRPC Reflection**: 有効（`grpc.reflection` パッケージ使用）
  - クライアントツール（grpcurl 等）からのサービス探索が可能
