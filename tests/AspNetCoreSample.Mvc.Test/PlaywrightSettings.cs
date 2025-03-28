using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public static class PlaywrightSettings
{
    private static readonly string[] Args = { "--ignore-certificate-errors" };

    public static BrowserTypeLaunchOptions DefaultBrowserTypeLaunchOptions(float? slowMo = default, bool headless = true)
    {
        return new BrowserTypeLaunchOptions { Args = Args, SlowMo = slowMo, Headless = headless };
    }

    public static BrowserNewContextOptions DefaultBrowserNewContextOptions()
    {
        return new BrowserNewContextOptions() { Locale = "ja-JP" };
    }

    public static void SetDefaultBrowserContext(IBrowserContext context)
    {
        context.SetDefaultTimeout(60_000);
    }
}
