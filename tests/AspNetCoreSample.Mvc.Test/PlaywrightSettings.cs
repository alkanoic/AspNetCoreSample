using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public static class PlaywrightSettings
{
    private static readonly string[] Args = { "--ignore-certificate-errors" };

    public static BrowserTypeLaunchOptions DefaultBrowserTypeLaunchOptions()
    {
        return new BrowserTypeLaunchOptions { Args = Args };
    }
}
