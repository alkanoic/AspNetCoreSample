import { test, expect } from '@playwright/test';
import { Prisma, PrismaClient } from '@prisma/client';

test('test', async ({ page }) => {
  await page.goto('https://localhost:7079/');
  await expect(page).toHaveScreenshot('test-2-1.png');
  await page.getByRole('list').getByRole('link', { name: 'Privacy' }).click();
  await expect(page).toHaveScreenshot('test-2-2.png');
});
