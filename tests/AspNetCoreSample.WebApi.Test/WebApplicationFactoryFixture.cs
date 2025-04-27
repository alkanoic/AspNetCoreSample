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

namespace AspNetCoreSample.WebApi.Test;

public sealed class WebApplicationFactoryFixture<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
{

    private readonly PostgreSqlContainer _postgresqlContainer;
    private readonly IContainer _keycloakContainer;

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

    public string KeycloakBaseAddress => new UriBuilder(Uri.UriSchemeHttp, _keycloakContainer.Hostname, _keycloakContainer.GetMappedPublicPort(KeycloakBuilder.KeycloakPort)).ToString();

    public string HostUrl { get; private set; } = "";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        HostUrl = $"https://localhost:{AvailablePort.GetAvailablePort()}";

        // 環境変数による設定の上書きはほかのテストに影響するため、InMemoryCollectionを使う
        builder.UseSetting("ConnectionStrings:Default", DbConnectionString);
        builder.UseSetting("KeycloakOptions:Authority", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test").ToString());
        builder.UseSetting("KeycloakOptions:TokenEndpoint", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test/protocol/openid-connect/token").ToString());
        builder.UseSetting("KeycloakOptions:RevokeTokenEndpoint", new Uri(new Uri(KeycloakBaseAddress), "/realms/Test/protocol/openid-connect/revoke").ToString());
        builder.UseSetting("KeycloakOptions:AdminTokenEndpoint", new Uri(new Uri(KeycloakBaseAddress), "/realms/master/protocol/openid-connect/token").ToString());
        builder.UseSetting("KeycloakOptions:AdminBaseAddress", KeycloakBaseAddress);
        builder.UseUrls(HostUrl);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var dummyHost = builder.Build();

        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        builder.Build().Start();

        return dummyHost;
    }
}
