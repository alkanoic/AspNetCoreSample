using DotNet.Testcontainers.Builders;

using Testcontainers.Keycloak;

using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace AspNetCoreSample.WebApi.Test;

public sealed class KeycloakFixture : IAsyncLifetime
{
    private readonly IContainer _keycloakContainer;
    private const int KeycloakPort = 8080;
    private const int KeycloakHealthPort = 9000;

    public KeycloakFixture()
    {
        _keycloakContainer = new ContainerBuilder()
            .WithImage("quay.io/keycloak/keycloak:latest")
            .WithResourceMapping("Test-realm.json", "/opt/keycloak/data/import/")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("LANG", "ja_JP.UTF-8")
            .WithEnvironment("KC_HEALTH_ENABLED", "true")
            .WithPortBinding(KeycloakPort, true)
            .WithPortBinding(KeycloakHealthPort, true)
            .WithCommand("start-dev")
            .WithCommand("--import-realm")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/health/ready").ForPort(KeycloakHealthPort)))
            .Build();
    }

    public string BaseAddress => new UriBuilder(Uri.UriSchemeHttp, _keycloakContainer.Hostname, _keycloakContainer.GetMappedPublicPort(KeycloakBuilder.KeycloakPort)).ToString();

    public Task InitializeAsync()
    {
        return _keycloakContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
