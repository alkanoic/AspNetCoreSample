using System.ComponentModel;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

using Npgsql;

using Testcontainers.PostgreSql;

using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace AspNetCoreSample.Mvc.Container.Test;

public sealed class MvcDbFixture : HttpClient
{
    private readonly INetwork _network;

    private readonly PostgreSqlContainer _postgresqlContainer;

    private static readonly X509Certificate Certificate = X509CertificateLoader.LoadPkcs12FromFile(MvcImage.CertificateFilePath, MvcImage.CertificatePassword);

    private static readonly MvcImage _mvcImage = new MvcImage();

    private readonly IContainer _mvcContainer;

    public MvcDbFixture() : base(new HttpClientHandler
    {
        // Trust the development certificate.
        ServerCertificateCustomValidationCallback = (_, certificate, _, _) => Certificate.Equals(certificate)
    })
    {
        _network = new NetworkBuilder().Build();

        _postgresqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithResourceMapping("migrate", "/docker-entrypoint-initdb.d")
            .WithEnvironment("TZ", "Asia/Tokyo")
            .WithEnvironment("POSTGRES_INITDB_ARGS", "--encoding=UTF-8")
            .WithNetwork(_network)
            .WithNetworkAliases(nameof(_postgresqlContainer))
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

        InitializeAsync().GetAwaiter().GetResult();
    }

    public const string DbConnectionString = $"Server={nameof(_postgresqlContainer)};Username={PostgreSqlBuilder.DefaultUsername};Password={PostgreSqlBuilder.DefaultPassword};Database={PostgreSqlBuilder.DefaultDatabase}";

    public static DbConnection DbConnection => new NpgsqlConnection(DbConnectionString);

    private async Task InitializeAsync()
    {
        await _mvcImage.InitializeAsync().ConfigureAwait(false);
        await _network.CreateAsync().ConfigureAwait(false);
        await _postgresqlContainer.StartAsync().ConfigureAwait(false);
        await _mvcContainer.StartAsync().ConfigureAwait(false);
    }

    protected override void Dispose(bool disposing)
    {
        // We do not need to manually dispose Docker resources. If resources depend on each
        // other, it is necessary to dispose them in the correct order. Testcontainers'
        // Resource Reaper (Ryuk) will reliably take care of these resources and dispose
        // them after the test automatically.
        if (disposing)
        {
            _mvcImage.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(30));
            _mvcContainer.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(30));
            _postgresqlContainer.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(30));
            _network.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(30));
        }

        base.Dispose(disposing);
    }

    public void SetBaseAddress()
    {
        var uriBuilder = new UriBuilder("https", _mvcContainer.Hostname, _mvcContainer.GetMappedPublicPort(MvcImage.HttpsPort));
        BaseAddress = uriBuilder.Uri;
    }
}
