import { test, expect, chromium } from "@playwright/test";

/**
 * 拡張機能を指定して Chrome を起動し、テストを実行するサンプル。
 *
 * 拡張機能はヘッドレスでは動作しないため、headless: false（ヘッド付き）で起動する。
 * 拡張機能のパスは環境変数 EXTENSIONS_DIR で切り替える（既定は Windows の Chrome プロファイル）。
 *
 * 実行例:
 *   npx playwright test launch-with-extensions.spec.ts
 *   EXTENSIONS_DIR="C:\path\to\extensions" npx playwright test launch-with-extensions.spec.ts
 */
test("拡張機能を読み込んで Chrome を起動する", async () => {
  const extDir =
    process.env.EXTENSIONS_DIR ??
    "C:\\Users\\naoto\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Extensions";

  const extensions = [
    `${extDir}\\ghbmnnjooekpmoecnnnilnnbdlolhkhi\\1.109.1_0`,
    `${extDir}\\nhdogjmejiglipccpnnnanhbledajbpd\\7.7.7_0`,
    `${extDir}\\nmmhkkegccagdldgiimedpiccmgmieda\\1.0.0.6_0`,
  ];

  const context = await chromium.launchPersistentContext("", {
    channel: "chrome",
    headless: false,
    args: [
      `--disable-extensions-except=${extensions.join(",")}`,
      ...extensions.map((e) => `--load-extension=${e}`),
    ],
  });

  const page = context.pages()[0] ?? (await context.newPage());

  await page.goto("https://www.google.com/");
  await expect(page).toHaveTitle(/Google/);

  await context.close();
});
