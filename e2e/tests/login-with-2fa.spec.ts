import { test, expect, chromium, type BrowserContext } from "@playwright/test";
import { existsSync } from "node:fs";

/**
 * 2FA が必要な既存サイトへのログインを「手入力」と「セッション再利用」で自動化するサンプル。
 *
 * 方針:
 * 1. 初回（auth.json が無い）: パスワードまで自動入力し、2FA は人間が手入力。
 *    ログイン完了（URL 遷移）を waitForURL で検知して、セッションを auth.json に保存する。
 * 2. 2回目以降（auth.json がある）: 保存したセッションを再利用し、2FA をスキップする。
 *
 * 環境変数:
 *   LOGIN_URL    ログインページの URL（既定 https://example.com/login）
 *   LOGIN_AFTER  ログイン成功後の URL パターン（既定 **/dashboard）
 *   USERNAME     ユーザー名
 *   PASSWORD     パスワード
 *   AUTH_FILE    セッション保存先（既定 auth.json）
 */

const AUTH_FILE = process.env.AUTH_FILE ?? "auth.json";
const LOGIN_URL = process.env.LOGIN_URL ?? "https://example.com/login";
const LOGIN_AFTER = process.env.LOGIN_AFTER ?? "**/dashboard";

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
    const userDataDir = `${process.env.TEMP ?? "/tmp"}\\chrome-debug-profile`;
    return await chromium.launchPersistentContext(userDataDir, {
      channel: "chrome",
      headless: false,
      args: ["--remote-debugging-port=9222"],
    });
  }
}

test("2FA を手入力で突破し、セッションを保存する", async () => {
  // 既にセッションが保存済みなら、このテストはスキップする
  test.skip(existsSync(AUTH_FILE), "auth.json が存在するためログイン不要");

  const context = await launchAndConnect();
  const page = context.pages()[0] ?? (await context.newPage());

  // パスワードまでは自動入力
  await page.goto(LOGIN_URL);
  await page.getByLabel("ユーザー名").fill(process.env.USERNAME!);
  await page.getByLabel("パスワード").fill(process.env.PASSWORD!);
  await page.getByRole("button", { name: "ログイン" }).click();

  // 2FA は人間が手入力。ログイン成功後の URL に遷移するまで待つ
  await page.waitForURL(LOGIN_AFTER, { timeout: 120_000 });

  // ログイン済みセッションを保存（以後のテストで再利用）
  await context.storageState({ path: AUTH_FILE });
});

test("保存したセッションでログイン済み状態を確認する", async () => {
  // セッションが無い場合は、先にログインテストを実行する必要がある
  test.skip(!existsSync(AUTH_FILE), "auth.json が存在しないため先にログインが必要");

  const context = await launchAndConnect();
  await context.storageState({ path: AUTH_FILE });

  const page = context.pages()[0] ?? (await context.newPage());
  await page.goto(LOGIN_AFTER.replace("**", "https://example.com"));

  // ログイン済みであることを検証（例: ダッシュボードのタイトル）
  await expect(page).toHaveTitle(/ダッシュボード/);
});
