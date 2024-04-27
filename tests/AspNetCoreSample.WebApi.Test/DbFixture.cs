using System.ComponentModel;
using System.Data.Common;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

using MySqlConnector;

using Testcontainers.MySql;

using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace AspNetCoreSample.Mvc.Test;

public sealed class DbFixture : IAsyncLifetime
{
    private readonly INetwork _network = new NetworkBuilder().Build();

    private readonly MySqlContainer _mySqlContainer;

    public DbFixture()
    {
        // Testcontainers starts the dependent database (PostgreSQL) and the database
        // migration tool Flyway. It establishes a network connection between these two
        // containers. Before starting the Flyway container, Testcontainers copies the SQL
        // migration files into it. When the Flyway container starts, it initiates the
        // dependent database container, connects to it, and begins the database migration
        // as soon as the database is ready. Once the migration is finished, the Flyway
        // container exits, and the database container becomes available for tests.

        _mySqlContainer = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithResourceMapping("my.cnf", "/etc/mysql/conf.d/my.cnf")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("LANG", "ja_JP.UTF-8")
            .WithNetwork(_network)
            .WithNetworkAliases(nameof(_mySqlContainer))
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
        // We do not need to manually dispose Docker resources. If resources depend on each
        // other, it is necessary to dispose them in the correct order. Testcontainers'
        // Resource Reaper (Ryuk) will reliably take care of these resources and dispose
        // them after the test automatically.
        return Task.CompletedTask;
    }

    private sealed class MigrationCompleted : IWaitUntil
    {
        // The Flyway container will exit after executing the database migration. We do not
        // check if the migration was successful. To verify its success, we can either
        // check the exit code of the container or the console output, respectively the
        // standard output (stdout) or error output (stderr).
        public Task<bool> UntilAsync(IContainer container)
        {
            return Task.FromResult(TestcontainersStates.Exited.Equals(container.State));
        }
    }
}
