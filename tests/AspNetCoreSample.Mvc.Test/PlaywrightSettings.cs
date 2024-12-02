using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public static class PlaywrightSettings
{
    private static readonly string[] Args = { "--ignore-certificate-errors" };

    public static BrowserTypeLaunchOptions DefaultBrowserTypeLaunchOptions()
    {
        return new BrowserTypeLaunchOptions { Args = Args };
        // return new BrowserTypeLaunchOptions { Args = Args, Headless = false };
    }

    public static BrowserNewContextOptions DefaultBrowserNewContextOptions()
    {
        return new BrowserNewContextOptions();
    }

    public static void SetDefaultBrowserContext(IBrowserContext context)
    {
        context.SetDefaultTimeout(60_000);
    }
}
