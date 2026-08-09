using AspNetCoreSample.Grpc;
using AspNetCoreSample.Grpc.Services;

using Grpc.Core;
using Grpc.Core.Testing;

using Microsoft.Extensions.Logging;

namespace AspNetCoreSample.Grpc.Test;

public sealed class GreeterServiceTest
{
    [Fact]
    [Trait("Category", nameof(GreeterServiceTest))]
    public async Task SayHelloReturnsGreeting()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<GreeterService>();
        var service = new GreeterService(logger);

        var request = new HelloRequest { Name = "World" };
        var context = TestServerCallContext.Create(
            method: nameof(GreeterService.SayHello),
            host: "localhost",
            deadline: DateTime.UtcNow.AddMinutes(30),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: "test",
            authContext: new AuthContext(string.Empty, new Dictionary<string, List<AuthProperty>>()),
            contextPropagationToken: null,
            writeHeadersFunc: _ => Task.CompletedTask,
            writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: _ => { }
        );

        var response = await service.SayHello(request, context);

        Assert.Equal("Hello World", response.Message);
    }

    [Fact]
    [Trait("Category", nameof(GreeterServiceTest))]
    public async Task SayHelloWithEmptyNameReturnsHello()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<GreeterService>();
        var service = new GreeterService(logger);

        var request = new HelloRequest { Name = "" };
        var context = TestServerCallContext.Create(
            method: nameof(GreeterService.SayHello),
            host: "localhost",
            deadline: DateTime.UtcNow.AddMinutes(30),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: "test",
            authContext: new AuthContext(string.Empty, new Dictionary<string, List<AuthProperty>>()),
            contextPropagationToken: null,
            writeHeadersFunc: _ => Task.CompletedTask,
            writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: _ => { }
        );

        var response = await service.SayHello(request, context);

        Assert.Equal("Hello ", response.Message);
    }

    [Fact]
    [Trait("Category", nameof(GreeterServiceTest))]
    public async Task SayHelloWithJapaneseNameReturnsGreeting()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<GreeterService>();
        var service = new GreeterService(logger);

        var request = new HelloRequest { Name = "太郎" };
        var context = TestServerCallContext.Create(
            method: nameof(GreeterService.SayHello),
            host: "localhost",
            deadline: DateTime.UtcNow.AddMinutes(30),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: "test",
            authContext: new AuthContext(string.Empty, new Dictionary<string, List<AuthProperty>>()),
            contextPropagationToken: null,
            writeHeadersFunc: _ => Task.CompletedTask,
            writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: _ => { }
        );

        var response = await service.SayHello(request, context);

        Assert.Equal("Hello 太郎", response.Message);
    }
}
