import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  await page.goto('http://localhost:5182/');
  await page.getByRole('link', { name: 'Home' }).click();
  await page.getByRole('list').getByRole('link', { name: 'Privacy' }).click();
});
