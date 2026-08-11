using System.Net;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreSample.Grpc.Test;

public sealed class ProgramTest : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTest()
    {
        _factory = new WebApplicationFactory<Program>();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Test]
    [Category(nameof(ProgramTest))]
    public async Task RootReturnsGrpcInstructionMessage()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/", CancellationToken.None);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);
        await Assert.That(body).Contains("Communication with gRPC endpoints");
    }

    [Test]
    [Category(nameof(ProgramTest))]
    public async Task AppUsesDevelopmentEnvironment()
    {
        var env = _factory.Services.GetRequiredService<IHostEnvironment>();

        await Assert.That(env.EnvironmentName).IsEqualTo(Environments.Development);
    }
}
