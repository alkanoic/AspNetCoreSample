import { test, expect, chromium, type BrowserContext } from "@playwright/test";

/**
 * 起動中の Chrome に接続してテストを実行するサンプル。
 *
 * 接続先は環境変数 CDP_ENDPOINT で切り替える。
 * - Windows で実行し Windows の Chrome に接続: 未指定（既定 http://localhost:9222）
 * - WSL で実行し Windows の Chrome に接続: 未指定（WSL2 の localhost 転送で localhost:9222 に届く）
 * - devcontainer で実行し Windows の Chrome に接続: CDP_ENDPOINT=http://<WIN_HOST>:9223（Windows 直上プロキシ経由）
 *
 * 実行例:
 *   npx playwright test connect-chrome.spec.ts
 *   CDP_ENDPOINT=http://172.17.160.1:9223 npx playwright test connect-chrome.spec.ts
 */

/**
 * Chrome に接続する。既にデバッグポート 9222 で起動していれば接続し、
 * 起動していなければデバッグポート付きで Chrome を起動してから接続する。
 */
async function launchAndConnect(): Promise<BrowserContext> {
  const endpoint = process.env.CDP_ENDPOINT ?? "http://localhost:9222";

  try {
    const browser = await chromium.connectOverCDP(endpoint);
    return browser.contexts()[0];
  } catch {
    // 起動していない場合は、デバッグポート付きで Chrome を起動する
    const userDataDir = `${process.env.TEMP ?? "/tmp"}\\chrome-debug-profile`;
    return await chromium.launchPersistentContext(userDataDir, {
      channel: "chrome",
      headless: false,
      args: ["--remote-debugging-port=9222"],
    });
  }
}

test("起動中の Chrome に接続して Google を表示する", async () => {
  const context = await launchAndConnect();
  const page = context.pages()[0] ?? (await context.newPage());

  await page.goto("https://www.google.com/");
  await expect(page).toHaveTitle(/Google/);
});
