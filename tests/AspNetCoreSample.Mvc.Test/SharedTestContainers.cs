using System.Data.Common;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

using Npgsql;

using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;

namespace AspNetCoreSample.Mvc.Test;

public static class SharedTestContainers
{
    private static readonly Lazy<Task> _initializeLazy = new(async () =>
    {
        PostgreSql = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("POSTGRES_INITDB_ARGS", "--encoding=UTF-8")
            .Build();

        Keycloak = new ContainerBuilder()
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

        await Task.WhenAll(Keycloak.StartAsync(), PostgreSql.StartAsync());
    });

    public static PostgreSqlContainer PostgreSql { get; private set; } = null!;

    public static IContainer Keycloak { get; private set; } = null!;

    public static string DbConnectionString => PostgreSql.GetConnectionString();

    public static DbConnection DbConnection => new NpgsqlConnection(DbConnectionString);

    public static string KeycloakBaseAddress => new UriBuilder(Uri.UriSchemeHttp, Keycloak.Hostname, Keycloak.GetMappedPublicPort(KeycloakBuilder.KeycloakPort)).ToString();

    public static Task InitializeAsync() => _initializeLazy.Value;

    public static async Task ResetNameTableAsync()
    {
        await using var conn = new NpgsqlConnection(DbConnectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM name; ALTER SEQUENCE name_id_seq RESTART WITH 1; INSERT INTO name (name) VALUES ('太郎'),('花子'),('令和');";
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task DisposeAsync()
    {
        if (Keycloak is not null)
        {
            await Keycloak.DisposeAsync();
        }

        if (PostgreSql is not null)
        {
            await PostgreSql.DisposeAsync();
        }
    }
}
