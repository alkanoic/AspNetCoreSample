using System.Data.Common;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Npgsql;

namespace AspNetCoreSample.WebApi.Test;

public sealed class WebApplicationFactoryFixture<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private IHost? _kestrelHost;
    private bool _disposed;

    public WebApplicationFactoryFixture()
    {
        SharedTestContainers.InitializeAsync().GetAwaiter().GetResult();
    }

    public string DbConnectionString => SharedTestContainers.DbConnectionString;

    public DbConnection DbConnection => new NpgsqlConnection(DbConnectionString);

    public string KeycloakBaseAddress => SharedTestContainers.KeycloakBaseAddress;

    public string HostUrl { get; private set; } = "";

    public override async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_kestrelHost != null)
        {
            await _kestrelHost.StopAsync();
            _kestrelHost.Dispose();
        }

        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseUrls("https://127.0.0.1:0");

        builder.UseSetting("ConnectionStrings:Default", DbConnectionString);
        builder.UseSetting("KeycloakOptions:Authority", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test").ToString());
        builder.UseSetting("KeycloakOptions:TokenEndpoint", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test/protocol/openid-connect/token").ToString());
        builder.UseSetting("KeycloakOptions:RevokeTokenEndpoint", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test/protocol/openid-connect/revoke").ToString());
        builder.UseSetting("KeycloakOptions:AdminTokenEndpoint", new Uri(new Uri(KeycloakBaseAddress), "/realms/master/protocol/openid-connect/token").ToString());
        builder.UseSetting("KeycloakOptions:AdminBaseAddress", KeycloakBaseAddress);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var dummyHost = builder.Build();
        dummyHost.StartAsync().GetAwaiter().GetResult();

        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        _kestrelHost = builder.Build();
        _kestrelHost.Start();

        HostUrl = ResolveHostUrl(_kestrelHost);

        return dummyHost;
    }

    private static string ResolveHostUrl(IHost host)
    {
        var addresses = host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
        var address = addresses?.Addresses.FirstOrDefault(a => a.StartsWith("https://", StringComparison.OrdinalIgnoreCase));

        for (var i = 0; address == null && i < 50; i++)
        {
            Task.Delay(100).GetAwaiter().GetResult();
            address = addresses?.Addresses.FirstOrDefault(a => a.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
        }

        return address ?? throw new InvalidOperationException("Kestrel のバインド先アドレスを解決できませんでした。");
    }
}
