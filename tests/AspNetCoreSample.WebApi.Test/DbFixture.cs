using System.Data.Common;

using MySqlConnector;

using Testcontainers.MySql;

namespace AspNetCoreSample.WebApi.Test;

public sealed class DbFixture : IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer;

    public DbFixture()
    {
        _mySqlContainer = new MySqlBuilder()
            .WithImage("mysql:latest")
            .WithResourceMapping("my.cnf", "/etc/mysql/conf.d/my.cnf")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("LANG", "ja_JP.UTF-8")
            .Build();
    }

    public string DbConnectionString => _mySqlContainer.GetConnectionString();

    public DbConnection DbConnection => new MySqlConnection(DbConnectionString);

    public Task InitializeAsync()
    {
        return _mySqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
