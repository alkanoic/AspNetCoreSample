using System.Diagnostics;

namespace AspNetCoreSample.Spa.Test;

// WebAPI（Testcontainers + Kestrel）と SPA（Nuxt dev サーバー）をまとめて起動し、同じフィクスチャで寿命を管理する。
public sealed class SpaTestFixture : IAsyncLifetime
{
    private readonly WebApplicationFactoryFixture<Program> _webFactory = new();
    private readonly object _logLock = new();
    private Process? _spaProcess;
    private StreamWriter? _spaLog;

    public string WebApiUrl => _webFactory.HostUrl;

    public string SpaUrl { get; } = "http://localhost:3000";

    public static HttpClient CreateWebApiClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        };

        return new HttpClient(handler);
    }

    public async ValueTask InitializeAsync()
    {
        await ((IAsyncLifetime)_webFactory).InitializeAsync();
        _webFactory.CreateDefaultClient();

        var logDir = Path.Combine(AppContext.BaseDirectory, "spa-logs");
        Directory.CreateDirectory(logDir);
        _spaLog = File.AppendText(Path.Combine(logDir, "nuxt.log"));
        _spaLog.AutoFlush = true;

        var startInfo = new ProcessStartInfo
        {
            FileName = "pnpm",
            Arguments = "dev --port 3000 --host 0.0.0.0",
            WorkingDirectory = FindNuxtDir(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        startInfo.Environment["API_BASE_URL"] = WebApiUrl;
        startInfo.Environment["NUXT_PUBLIC_KEYCLOAK_URL"] = _webFactory.KeycloakBaseAddress;

        _spaProcess = Process.Start(startInfo) ?? throw new InvalidOperationException("Nuxt の起動に失敗しました。");
        _spaProcess.OutputDataReceived += (_, e) => WriteLog(e.Data);
        _spaProcess.ErrorDataReceived += (_, e) => WriteLog(e.Data);
        _spaProcess.BeginOutputReadLine();
        _spaProcess.BeginErrorReadLine();

        await WaitForReadyAsync();
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_spaProcess is { HasExited: false })
            {
                _spaProcess.Kill(entireProcessTree: true);
                await _spaProcess.WaitForExitAsync();
            }
        }
        catch (InvalidOperationException)
        {
            // 終了済み
        }
        finally
        {
            _spaProcess?.Dispose();
            _spaProcess = null;
            _spaLog?.Dispose();
            _spaLog = null;
        }

        await _webFactory.DisposeAsync();
    }

    private void WriteLog(string? data)
    {
        if (data == null)
        {
            return;
        }

        lock (_logLock)
        {
            _spaLog?.WriteLine(data);
        }
    }

    private async Task WaitForReadyAsync()
    {
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var deadline = DateTimeOffset.UtcNow.AddMinutes(8);

        while (true)
        {
            try
            {
                using var response = await httpClient.GetAsync($"{SpaUrl}/login");
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // 起動待ち
            }
            catch (TaskCanceledException)
            {
                // 起動待ち
            }

            if (DateTimeOffset.UtcNow >= deadline)
            {
                throw new TimeoutException($"Nuxt が起動しませんでした。({SpaUrl})");
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }

    private static string FindNuxtDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "NuxtSample");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("src/NuxtSample が見つかりません。");
    }
}
