import { test, expect, chromium } from '@playwright/test';

/**
 * 起動中の Chrome に接続してテストを実行するサンプル。
 *
 * 接続先は環境変数 CDP_ENDPOINT で切り替える。
 * - Windows で実行し Windows の Chrome に接続: 未指定（既定 http://localhost:9222）
 * - WSL で実行し Windows の Chrome に接続: CDP_ENDPOINT=http://<WIN_HOST>:9222
 *
 * 実行例:
 *   npx playwright test connect-chrome.spec.ts
 *   CDP_ENDPOINT=http://172.20.0.1:9222 npx playwright test connect-chrome.spec.ts
 */
test('起動中の Chrome に接続して Google を表示する', async () => {
  const endpoint = process.env.CDP_ENDPOINT ?? 'http://localhost:9222';
  const browser = await chromium.connectOverCDP(endpoint);

  const context = browser.contexts()[0];
  const page = context.pages()[0] ?? await context.newPage();

  await page.goto('https://www.google.com/');
  await expect(page).toHaveTitle(/Google/);
});
