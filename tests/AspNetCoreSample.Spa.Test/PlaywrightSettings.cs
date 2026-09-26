using Microsoft.Playwright;

namespace AspNetCoreSample.Spa.Test;

public static class PlaywrightSettings
{
    private static readonly string[] Args = { "--ignore-certificate-errors", "--no-sandbox" };

    public static BrowserTypeLaunchOptions DefaultBrowserTypeLaunchOptions(float? slowMo = default, bool headless = true)
    {
        // headless shell ではなくフル版の Chromium を使う。headless shell は環境によって
        // NewPageAsync 時にクラッシュすることがある
        return new BrowserTypeLaunchOptions { Channel = "chromium", Args = Args, SlowMo = slowMo, Headless = headless };
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
