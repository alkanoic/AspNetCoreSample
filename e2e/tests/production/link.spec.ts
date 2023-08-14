import { test, expect } from '@playwright/test';

test('home and privacy link test', async ({ page }) => {
  await page.goto('/');
  const viewportSize = page.viewportSize();
  if (viewportSize.width <= 480) {
    // モバイルデバイスのコードを実行
    await page.getByLabel('Toggle navigation').click();
  }
  await expect(page).toHaveScreenshot('test-2-1.png');
  await page.getByRole('list').getByRole('link', { name: 'Privacy' }).click();
  await expect(page).toHaveScreenshot('test-2-2.png');
});
