using System.Data.Common;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Npgsql;

using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;

namespace AspNetCoreSample.WebApi.Test;

public sealed class WebApplicationFactoryFixture<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
{

    private readonly PostgreSqlContainer _postgresqlContainer;
    private readonly IContainer _keycloakContainer;
    private IHost? _kestrelHost;
    private bool _disposed;

    public WebApplicationFactoryFixture()
    {
        _postgresqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("POSTGRES_INITDB_ARGS", "--encoding=UTF-8")
            .Build();

        _keycloakContainer = new ContainerBuilder()
            .WithImage("quay.io/keycloak/keycloak:latest")
            .WithResourceMapping("Test-realm.json", "/opt/keycloak/data/import/")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("LANG", "ja_JP.UTF-8")
            .WithEnvironment("KC_HEALTH_ENABLED", "true")
            .WithEnvironment("KEYCLOAK_ADMIN", "admin")
            .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "passwd")
            .WithPortBinding(KeycloakBuilder.KeycloakPort, true)
            .WithPortBinding(KeycloakBuilder.KeycloakHealthPort, true)
            .WithCommand("start-dev")
            .WithCommand("--import-realm")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/health/ready").ForPort(KeycloakBuilder.KeycloakHealthPort)))
            .Build();
    }

    public string DbConnectionString => _postgresqlContainer.GetConnectionString();

    public DbConnection DbConnection => new NpgsqlConnection(DbConnectionString);

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await Task.WhenAll(_keycloakContainer.StartAsync(), _postgresqlContainer.StartAsync());
    }

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

        await _keycloakContainer.DisposeAsync();
        await _postgresqlContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    public string KeycloakBaseAddress => new UriBuilder(Uri.UriSchemeHttp, _keycloakContainer.Hostname, _keycloakContainer.GetMappedPublicPort(KeycloakBuilder.KeycloakPort)).ToString();

    public string HostUrl { get; private set; } = "";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseUrls("https://127.0.0.1:0");

        // 環境変数による設定の上書きはほかのテストに影響するため、InMemoryCollectionを使う
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

        // ポート 0 の場合、バインド先の確定を待つ必要がある
        for (var i = 0; address == null && i < 50; i++)
        {
            Task.Delay(100).GetAwaiter().GetResult();
            address = addresses?.Addresses.FirstOrDefault(a => a.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
        }

        return address ?? throw new InvalidOperationException("Kestrel のバインド先アドレスを解決できませんでした。");
    }
}
