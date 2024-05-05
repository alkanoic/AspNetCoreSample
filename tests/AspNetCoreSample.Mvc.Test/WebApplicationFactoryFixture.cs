using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreSample.Mvc.Test;

public class WebApplicationFactoryFixture<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    public string HostUrl { get; private set; } = "";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        HostUrl = $"https://localhost:{AvailablePort.GetAvailablePort()}";
        builder.UseUrls(HostUrl);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var dummyHost = builder.Build();

        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        builder.Build().Start();

        return dummyHost;
    }
}
