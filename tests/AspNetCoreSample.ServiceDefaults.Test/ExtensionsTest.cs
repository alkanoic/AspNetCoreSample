using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreSample.ServiceDefaults.Test;

public sealed class ExtensionsTest
{
    [Fact]
    [Trait("Category", nameof(ExtensionsTest))]
    public async Task AddDefaultHealthChecks_RegistersHealthCheck()
    {
        var builder = WebApplication.CreateBuilder(["--environment", "Development"]);
        builder.WebHost.UseTestServer();
        builder.AddDefaultHealthChecks();

        var app = builder.Build();
        app.MapDefaultEndpoints();

        await app.StartAsync(TestContext.Current.CancellationToken);
        try
        {
            using var client = ((IHost)app).GetTestClient();
            var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
        }
    }

    [Fact]
    [Trait("Category", nameof(ExtensionsTest))]
    public async Task MapDefaultEndpoints_ReturnsHealthyForAlive()
    {
        var builder = WebApplication.CreateBuilder(["--environment", "Development"]);
        builder.WebHost.UseTestServer();
        builder.AddDefaultHealthChecks();

        var app = builder.Build();
        app.MapDefaultEndpoints();

        await app.StartAsync(TestContext.Current.CancellationToken);
        try
        {
            using var client = ((IHost)app).GetTestClient();
            var response = await client.GetAsync("/alive", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
        }
    }

    [Fact]
    [Trait("Category", nameof(ExtensionsTest))]
    public async Task AddServiceDefaults_ConfiguresServices()
    {
        var builder = WebApplication.CreateBuilder(["--environment", "Development"]);
        builder.WebHost.UseTestServer();
        builder.AddServiceDefaults();

        var app = builder.Build();
        app.MapDefaultEndpoints();

        await app.StartAsync(TestContext.Current.CancellationToken);
        try
        {
            using var client = ((IHost)app).GetTestClient();
            var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
        }
    }
}
