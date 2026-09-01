import { chromium } from "@playwright/test";

/**
 * デバッグポートで起動中の Chrome に接続し、現在開いているタブの
 * アクセシビリティツリー（ariaSnapshot）を取得する。
 *
 * 使い方:
 *   npx tsx aria-snapshot.ts            # 現在のタブを解析
 *   npx tsx aria-snapshot.ts <URL>      # 指定 URL を開いて解析
 */
async function main() {
  const endpoint = process.env.CDP_ENDPOINT ?? "http://localhost:9222";
  const url = process.argv[2];

  const browser = await chromium.connectOverCDP(endpoint);
  const context = browser.contexts()[0];
  const page = context.pages()[0] ?? (await context.newPage());

  if (url) {
    await page.goto(url);
  }

  const snapshot = await page.locator("body").ariaSnapshot();
  console.log(snapshot);
}

main();
