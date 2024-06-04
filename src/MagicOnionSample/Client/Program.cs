using Grpc.Net.Client;
using MagicOnion.Client;
using AspNetCoreSample.MagicOnion.Services;

var handler = new HttpClientHandler();
// 証明書無視
handler.ServerCertificateCustomValidationCallback =
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

// Connect to the server using gRPC channel.
var channel = GrpcChannel.ForAddress("https://localhost:7082", new GrpcChannelOptions { HttpHandler = handler });

// Create a proxy to call the server transparently.
var client = MagicOnionClient.Create<IMyFirstService>(channel);

// Call the server-side method using the proxy.
var result = await client.SumAsync(123, 456);
Console.WriteLine($"Result: {result}");
