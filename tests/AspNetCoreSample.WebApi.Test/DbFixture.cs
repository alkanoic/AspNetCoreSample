using System.Data.Common;

using Npgsql;

using Testcontainers.PostgreSql;

namespace AspNetCoreSample.WebApi.Test;

public sealed class DbFixture : IAsyncDisposable
{
    private readonly PostgreSqlContainer _postgresqlContainer;

    public DbFixture()
    {
        _postgresqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("POSTGRES_INITDB_ARGS", "--encoding=UTF-8")
            .Build();

        _postgresqlContainer.StartAsync().GetAwaiter().GetResult();
    }

    public string DbConnectionString => _postgresqlContainer.GetConnectionString();

    public DbConnection DbConnection => new NpgsqlConnection(DbConnectionString);

    public async ValueTask DisposeAsync()
    {
        await _postgresqlContainer.DisposeAsync();
    }
}
