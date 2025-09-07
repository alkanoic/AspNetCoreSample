using System.Data.Common;

using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Npgsql;

using Testcontainers.PostgreSql;

using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace DbContainer.Test;

public sealed class PostgresFixture
{
    private readonly PostgreSqlContainer _postgresqlContainer;

    public PostgresFixture()
    {
        _postgresqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("POSTGRES_INITDB_ARGS", "--encoding=UTF-8")
            .Build();
    }

    public string DbConnectionString => _postgresqlContainer.GetConnectionString();

    public DbConnection DbConnection => new NpgsqlConnection(DbConnectionString);

    public async Task InitializeAsync()
    {
        await _postgresqlContainer.StartAsync();
    }

    public ValueTask DisposeAsync()
        => new ValueTask(_postgresqlContainer.DisposeAsync().AsTask());

    private sealed class MigrationCompleted : IWaitUntil
    {
        public Task<bool> UntilAsync(IContainer container)
        {
            return Task.FromResult(TestcontainersStates.Exited.Equals(container.State));
        }
    }
}
