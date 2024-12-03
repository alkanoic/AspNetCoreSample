using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public static class PlaywrightSettings
{
    private static readonly string[] Args = { "--ignore-certificate-errors" };

    public static BrowserTypeLaunchOptions DefaultBrowserTypeLaunchOptions(float? slowMo = default)
    {
        var options = new BrowserTypeLaunchOptions { Args = Args, SlowMo = slowMo };
        return options;
        // options.Headless = false;
        // return options;
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
