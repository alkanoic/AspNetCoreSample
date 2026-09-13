import { defineConfig } from '@playwright/test';
import baseConfig from './playwright.config';

/**
 * 起動中の Chrome に接続するテスト専用の設定。
 * 接続先は単一の Chrome インスタンスのため、プロジェクトは chromium のみ・並列なしで実行する。
 */
const connectConfig = defineConfig({
  workers: 1,
  testMatch: 'connect-chrome.spec.ts',
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
  ...connectConfig,
});
