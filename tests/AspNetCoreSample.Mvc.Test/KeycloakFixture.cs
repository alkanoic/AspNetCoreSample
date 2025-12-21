using DotNet.Testcontainers.Containers;

using Testcontainers.Keycloak;

namespace AspNetCoreSample.WebApi.Test;

public sealed class KeycloakFixture
{
    private readonly IContainer _keycloakContainer;
    private const int KeycloakPort = 8080;

    public KeycloakFixture()
    {
        _keycloakContainer = new KeycloakBuilder()
            .WithImage("quay.io/keycloak/keycloak:21.0")
            .WithResourceMapping("Test-realm.json", "/opt/keycloak/data/import/")
            .WithUsername("admin")
            .WithPassword("admin")
            .WithPortBinding(KeycloakPort, true)
            .WithCommand("start-dev", "--import-realm")
            .Build();

        Task.Run(async () => await InitializeAsync());
    }

    public string BaseAddress => new UriBuilder(Uri.UriSchemeHttp, _keycloakContainer.Hostname, _keycloakContainer.GetMappedPublicPort(KeycloakPort)).ToString();

    private async Task InitializeAsync()
    {
        await _keycloakContainer.StartAsync();
    }
}
