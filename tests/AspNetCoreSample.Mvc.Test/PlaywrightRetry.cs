using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

// CI では並行する Testcontainers の Docker ネットワーク構築が原因で Chromium が
// ナビゲーションを ERR_NETWORK_CHANGED で中断し、要素検索がタイムアウトすることがある（既知の flaky）。
// 該当エラーのときだけテスト本文を最初からやり直す。アサーション失敗は再試行しない。
public static class PlaywrightRetry
{
    public static async Task RunAsync(Func<Task> action, int maxRetries = 3)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                await action();
                return;
            }
            catch (TimeoutException) when (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            catch (PlaywrightException ex) when (attempt < maxRetries && ex.Message.Contains("ERR_NETWORK_CHANGED"))
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
