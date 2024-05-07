using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

using Testcontainers.Keycloak;

using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace AspNetCoreSample.WebApi.Test;

public sealed class KeycloakFixture : IAsyncLifetime
{
    private readonly INetwork _network = new NetworkBuilder().Build();

    private readonly KeycloakContainer _keycloakContainer;

    public KeycloakFixture()
    {
        _keycloakContainer = new KeycloakBuilder()
            .WithImage("quay.io/keycloak/keycloak:latest")
            .WithResourceMapping("Test-realm.json", "/opt/keycloak/data/import/")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("LANG", "ja_JP.UTF-8")
            .WithNetwork(_network)
            .WithNetworkAliases(nameof(_keycloakContainer))
            .WithCommand("--import-realm")
            .Build();
    }

    public string BaseAddress => _keycloakContainer.GetBaseAddress();

    public Task InitializeAsync()
    {
        return _keycloakContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private sealed class MigrationCompleted : IWaitUntil
    {
        public Task<bool> UntilAsync(IContainer container)
        {
            return Task.FromResult(TestcontainersStates.Exited.Equals(container.State));
        }
    }
}
