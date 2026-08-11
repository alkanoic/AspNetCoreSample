using System.Data.Common;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using Npgsql;

namespace AspNetCoreSample.Mvc.Test;

public class WebApplicationFactoryFixture<TEntryPoint> : WebApplicationFactory<TEntryPoint>
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
        GC.SuppressFinalize(this);

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

        builder.UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:Default", DbConnectionString},
                {"KeycloakOptions:Authority", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test").ToString()},
                {"KeycloakOptions:MetadataAddress", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test/.well-known/openid-configuration").ToString()},
            }).Build());

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();
            services.AddRazorComponents().AddInteractiveServerComponents();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var dummyHost = builder.Build();

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
