using System.Globalization;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

namespace AspNetCoreSample.Mvc.Container.Test;

public sealed class MvcImage : IImage, IAsyncLifetime, IDisposable
{
    public const ushort HttpsPort = 443;

    public const string CertificateFilePath = "certificate.crt";

    public const string CertificatePassword = "password";

    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

    private readonly DockerImage _image = new DockerImage("localhost/testcontainers", "aspnetcoresample-mvc", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));

    private string _tempDockerPath = "";

    public async Task InitializeAsync()
    {
        await _semaphoreSlim.WaitAsync()
          .ConfigureAwait(false);

        _tempDockerPath = CreateTempDockerfileWithoutPlatform(Path.Combine(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath, "Dockerfile"));
        var relativeDockerfile = Path.GetRelativePath(CommonDirectoryPath.GetSolutionDirectory().DirectoryPath, _tempDockerPath);

        try
        {
            await new ImageFromDockerfileBuilder()
              .WithName(this)
              .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
              .WithDockerfile(relativeDockerfile)
              .WithDeleteIfExists(false)
              .Build()
              .CreateAsync()
              .ConfigureAwait(false);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
    }

    public string Repository => _image.Repository;

    public string Name => _image.Name;

    public string Tag => _image.Tag;

    public string FullName => _image.FullName;

    public string GetHostname()
    {
        return _image.GetHostname();
    }

    private static string CreateTempDockerfileWithoutPlatform(string source)
    {
        var target = $"{source}.tmp";
        File.WriteAllText(target, File.ReadAllText(source).Replace("--platform=$BUILDPLATFORM", ""));
        return target;
    }
}
