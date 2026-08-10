using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public static class PlaywrightSettings
{
    private static readonly string[] Args = { "--ignore-certificate-errors", "--no-sandbox" };

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

    // CI では並行する Testcontainers の Docker ネットワーク構築が原因で Chromium が
    // ERR_NETWORK_CHANGED を返すことがある（既知の flaky）。該当エラー時のみリトライする。
    public static async Task GotoWithRetryAsync(IPage page, string url, int maxRetries = 3)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                await page.GotoAsync(url);
                return;
            }
            catch (PlaywrightException ex) when (ex.Message.Contains("ERR_NETWORK_CHANGED") && attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
