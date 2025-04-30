namespace AspNetCoreSample.Util.Logging;

public static class ServiceProviderAccessor
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public static void Initialize(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
