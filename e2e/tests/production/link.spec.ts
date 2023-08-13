import { test, expect } from '@playwright/test';

test('home and privacy link test', async ({ page }) => {
  await page.goto('/');
  await expect(page).toHaveScreenshot('test-2-1.png');
  await page.getByRole('list').getByRole('link', { name: 'Privacy' }).click();
  await expect(page).toHaveScreenshot('test-2-2.png');
});
