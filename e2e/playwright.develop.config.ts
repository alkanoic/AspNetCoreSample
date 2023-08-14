import { defineConfig } from '@playwright/test';
import baseConfig from './playwright.config';

const developConfig = defineConfig({
  workers: 1,
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: [
    ['line'],
    ['html', { outputFolder: 'playwright-report-develop' }],
    ['allure-playwright'],
  ],
  testMatch: 'develop/*',
  outputDir: 'test-results-develop',
});

export default defineConfig({
  ...baseConfig,
  ...developConfig,
});
