import { defineConfig } from '@playwright/test';
import baseConfig from './playwright.config';

/**
 * 2FA 手入力ログイン + セッション再利用のテスト専用設定。
 * 手入力（waitForURL）を伴うため、単一プロジェクト・並列なし・ヘッド付きで実行する。
 */
const loginConfig = defineConfig({
  workers: 1,
  testMatch: 'login-with-2fa.spec.ts',
  reporter: [['line']],
  projects: [
    {
      name: 'chromium',
      use: { ...baseConfig.projects?.[0]?.use },
    },
  ],
});

export default defineConfig({
  ...baseConfig,
  ...loginConfig,
});
