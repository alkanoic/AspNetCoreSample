import { defineConfig } from '@playwright/test';
import baseConfig from './playwright.config';

/**
 * 拡張機能を読み込んで Chrome を起動するテスト専用の設定。
 * 拡張機能は Chromium 系（chrome チャンネル）のみ対応のため、単一プロジェクト・並列なしで実行する。
 */
const extensionsConfig = defineConfig({
  workers: 1,
  testMatch: 'launch-with-extensions.spec.ts',
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
  ...extensionsConfig,
});
