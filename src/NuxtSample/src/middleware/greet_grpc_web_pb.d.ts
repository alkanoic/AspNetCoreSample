import * as grpcWeb from 'grpc-web';

import * as greet_pb from './greet_pb'; // proto import: "greet.proto"


export class GreeterClient {
  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; });

  sayHello(
    request: greet_pb.HelloRequest,
    metadata: grpcWeb.Metadata | undefined,
    callback: (err: grpcWeb.RpcError,
               response: greet_pb.HelloReply) => void
  ): grpcWeb.ClientReadableStream<greet_pb.HelloReply>;

}

export class GreeterPromiseClient {
  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; });

  sayHello(
    request: greet_pb.HelloRequest,
    metadata?: grpcWeb.Metadata
  ): Promise<greet_pb.HelloReply>;

}

