using System.Net;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreSample.Grpc.Test;

public sealed class ProgramTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    [Trait("Category", nameof(ProgramTest))]
    public async ValueTask RootReturnsGrpcInstructionMessage()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("Communication with gRPC endpoints", body);
    }

    [Fact]
    [Trait("Category", nameof(ProgramTest))]
    public async ValueTask AppUsesDevelopmentEnvironment()
    {
        var env = _factory.Services.GetRequiredService<IHostEnvironment>();

        Assert.Equal(Environments.Development, env.EnvironmentName);
    }
}
