using System.ComponentModel;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

using MySqlConnector;

using Testcontainers.MySql;

using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace AspNetCoreSample.Mvc.Container.Test;

public sealed class MvcDbFixture : HttpClient, IAsyncLifetime
{
    private readonly INetwork _network;

    private readonly MySqlContainer _mySqlContainer;

    private static readonly X509Certificate Certificate = new X509Certificate2(MvcImage.CertificateFilePath, MvcImage.CertificatePassword);

    private static readonly MvcImage _mvcImage = new MvcImage();

    private readonly IContainer _mvcContainer;

    public MvcDbFixture() : base(new HttpClientHandler
    {
        // Trust the development certificate.
        ServerCertificateCustomValidationCallback = (_, certificate, _, _) => Certificate.Equals(certificate)
    })
    {
        _network = new NetworkBuilder().Build();

        _mySqlContainer = new MySqlBuilder()
            .WithImage("mysql:latest")
            .WithResourceMapping("my.cnf", "/etc/mysql/conf.d/my.cnf")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("LANG", "ja_JP.UTF-8")
            .WithNetwork(_network)
            .WithNetworkAliases(nameof(_mySqlContainer))
            .Build();

        _mvcContainer = new ContainerBuilder()
            // .DependsOn(_mySqlContainer)
            .WithImage(_mvcImage)
            .WithNetwork(_network)
            .WithPortBinding(MvcImage.HttpsPort, true)
            .WithEnvironment("ASPNETCORE_URLS", "https://+")
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", MvcImage.CertificateFilePath)
            .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", MvcImage.CertificatePassword)
            .WithEnvironment("ConnectionStrings__DefaultConnection", DbConnectionString)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MvcImage.HttpsPort))
            .Build();
    }

    public const string DbConnectionString = $"Server={nameof(_mySqlContainer)};User={MySqlBuilder.DefaultUsername};Password={MySqlBuilder.DefaultPassword};Database={MySqlBuilder.DefaultDatabase}";

    public static DbConnection DbConnection => new MySqlConnection(DbConnectionString);

    public async Task InitializeAsync()
    {
        await _mvcImage.InitializeAsync().ConfigureAwait(false);
        await _network.CreateAsync().ConfigureAwait(false);
        await _mySqlContainer.StartAsync().ConfigureAwait(false);
        await _mvcContainer.StartAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        // We do not need to manually dispose Docker resources. If resources depend on each
        // other, it is necessary to dispose them in the correct order. Testcontainers'
        // Resource Reaper (Ryuk) will reliably take care of these resources and dispose
        // them after the test automatically.
        await _mvcImage.DisposeAsync().ConfigureAwait(false);
        await _mvcContainer.DisposeAsync().ConfigureAwait(false);
        await _mySqlContainer.DisposeAsync().ConfigureAwait(false);
        await _network.DisposeAsync().ConfigureAwait(false);
    }

    public void SetBaseAddress()
    {
        try
        {
            var uriBuilder = new UriBuilder("https", _mvcContainer.Hostname, _mvcContainer.GetMappedPublicPort(MvcImage.HttpsPort));
            BaseAddress = uriBuilder.Uri;
        }
        catch
        {
            // Set the base address only once.
        }
    }
}
