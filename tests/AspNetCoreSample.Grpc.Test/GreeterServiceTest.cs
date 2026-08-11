using AspNetCoreSample.Grpc;
using AspNetCoreSample.Grpc.Services;

using Grpc.Core;
using Grpc.Core.Testing;

using Microsoft.Extensions.Logging;

namespace AspNetCoreSample.Grpc.Test;

public sealed class GreeterServiceTest
{
    [Test]
    [Category(nameof(GreeterServiceTest))]
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

        await Assert.That(response.Message).IsEqualTo("Hello World");
    }

    [Test]
    [Category(nameof(GreeterServiceTest))]
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

        await Assert.That(response.Message).IsEqualTo("Hello ");
    }

    [Test]
    [Category(nameof(GreeterServiceTest))]
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

        await Assert.That(response.Message).IsEqualTo("Hello 太郎");
    }
}
