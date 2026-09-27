using Microsoft.Playwright;

namespace AspNetCoreSample.Spa.Test;

public static class PlaywrightSettings
{
    public static BrowserTypeLaunchOptions DefaultBrowserTypeLaunchOptions(float? slowMo = default, bool headless = true)
    {
        return new BrowserTypeLaunchOptions { Channel = "chrome", SlowMo = slowMo, Headless = headless };
    }

    public static BrowserNewContextOptions DefaultBrowserNewContextOptions()
    {
        return new BrowserNewContextOptions() { IgnoreHTTPSErrors = true, Locale = "ja-JP" };
    }

    public static void SetDefaultBrowserContext(IBrowserContext context)
    {
        context.SetDefaultTimeout(60_000);
    }
}
