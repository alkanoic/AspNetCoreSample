using Microsoft.Playwright;

namespace AspNetCoreSample.Spa.Test;

// CI では並行する Testcontainers の Docker ネットワーク構築が原因でブラウザーが
// ナビゲーションを ERR_NETWORK_CHANGED で中断することがある（既知の flaky）。
// このネットワークエラーだけ再試行し、操作タイムアウトは原因を確認できるようそのまま失敗させる。
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
            catch (PlaywrightException ex) when (attempt < maxRetries && ex.Message.Contains("ERR_NETWORK_CHANGED"))
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
