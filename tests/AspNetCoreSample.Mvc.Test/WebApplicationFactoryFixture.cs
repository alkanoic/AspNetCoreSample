using System.Data.Common;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Npgsql;

using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;

namespace AspNetCoreSample.Mvc.Test;

public class WebApplicationFactoryFixture<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private readonly PostgreSqlContainer _postgresqlContainer;
    private readonly KeycloakContainer _keycloakContainer;
    private readonly SemaphoreSlim _initializationSemaphore = new(1, 1);
    private bool _initialized;

    public WebApplicationFactoryFixture()
    {
        var sessionId = Guid.NewGuid().ToString("N")[..8];

        _postgresqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("POSTGRES_INITDB_ARGS", "--encoding=UTF-8")
            .WithName($"postgres-test-{sessionId}")
            .Build();

        // _keycloakContainer = new KeycloakBuilder()
        //     .WithResourceMapping("Test-realm.json", "/opt/keycloak/data/import/")
        //     .WithEnvironment("TZ", "Asia/Tokyo")
        //     .WithEnvironment("LANG", "ja_JP.UTF-8")
        //     .WithEnvironment("KC_HEALTH_ENABLED", "true")
        //     .WithEnvironment("KEYCLOAK_ADMIN", "admin")
        //     .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "passwd")
        //     .WithCommand("start-dev", "--import-realm")
        //     .WithName($"keycloak-test-{sessionId}")
        //     .Build();

        _keycloakContainer = new KeycloakBuilder()
            .WithImage("quay.io/keycloak/keycloak:latest")
            .WithResourceMapping("Test-realm.json", "/opt/keycloak/data/import/")
            .WithUsername("admin")
            .WithPassword("admin")
            .WithRealm("Test-realm.json")
            .WithEnvironment("LANG", "ja_JP.UTF-8")
            .WithEnvironment("LC_ALL", "ja_JP.UTF-8")
            // .WithPortBinding(KeycloakPort, true)
            // .WithCommand("start-dev", "--import-realm")
            .WithName($"keycloak-test-{sessionId}")
            .Build();
    }

    private async Task InitializeAsync()
    {
        await _initializationSemaphore.WaitAsync();
        try
        {
            if (!_initialized)
            {
                await Task.WhenAll(_keycloakContainer.StartAsync(), _postgresqlContainer.StartAsync());
                _initialized = true;
            }
        }
        finally
        {
            _initializationSemaphore.Release();
        }
    }

    public string DbConnectionString => _postgresqlContainer.GetConnectionString();

    public DbConnection DbConnection => new NpgsqlConnection(DbConnectionString);

    public string KeycloakBaseAddress => _keycloakContainer.GetBaseAddress();

    public string HostUrl { get; private set; } = "";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // コンテナの初期化を待つ
        InitializeAsync().GetAwaiter().GetResult();

        HostUrl = $"https://localhost:{AvailablePort.GetAvailablePort()}";

        // 環境変数による設定の上書きはほかのテストに影響するため、InMemoryCollectionを使う
        builder.UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:Default", DbConnectionString},
                {"KeycloakOptions:Authority", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test").ToString()},
                {"KeycloakOptions:MetadataAddress", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test/.well-known/openid-configuration").ToString()},
            }).Build());
        builder.UseUrls(HostUrl);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var dummyHost = builder.Build();

        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        var host = builder.Build();
        host.Start();

        return dummyHost;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                _keycloakContainer?.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(30));
            }
            catch { }

            try
            {
                _postgresqlContainer?.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(30));
            }
            catch { }

            _initializationSemaphore?.Dispose();
        }
        base.Dispose(disposing);
    }
}
