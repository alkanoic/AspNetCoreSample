import { defineConfig } from '@playwright/test';
import baseConfig from './playwright.config';

const developConfig = defineConfig({
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: [
    ['line'],
    ['html', { outputFolder: 'playwright-report-production' }],
    ['allure-playwright'],
  ],
  testMatch: 'production/*',
  outputDir: 'test-results-production',
});

export default defineConfig({
  ...baseConfig,
  ...developConfig,
});
