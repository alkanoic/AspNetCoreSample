using Microsoft.Playwright;

namespace AspNetCoreSample.Spa.Test;

// CI では並行する Testcontainers の Docker ネットワーク構築が原因でブラウザーが
// ナビゲーションを ERR_NETWORK_CHANGED で中断することがある（既知の flaky）。
// ページ初回表示で発生するこのネットワークエラーだけ再試行する。ログイン処理など状態を変える操作は再実行しない。
// 操作タイムアウトは原因を確認できるよう、そのまま失敗させる。
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
