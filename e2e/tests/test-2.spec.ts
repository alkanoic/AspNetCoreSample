import { test, expect } from '@playwright/test';
import { Prisma, PrismaClient } from '@prisma/client';

test('test', async ({ page }) => {
  await page.goto('http://localhost:5182/');
  await expect(page).toHaveScreenshot('test-2-1.png');
  await page.getByRole('list').getByRole('link', { name: 'Privacy' }).click();
  await expect(page).toHaveScreenshot('test-2-2.png');

  const prisma = new PrismaClient();
  const name_count = await prisma.name.count();
  expect(name_count).toBe(3);
  const name_first = await prisma.name.findFirst();
  expect(name_first?.name).toBe('太郎');
});
