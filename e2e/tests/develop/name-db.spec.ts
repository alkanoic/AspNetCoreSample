import { test, expect } from '@playwright/test';
import { PrismaClient } from '@prisma/client';

test('test', async ({ page }) => {
  await page.goto('/');
  await page.getByRole('link', { name: 'Name' }).click();
  await expect(page).toHaveScreenshot('name-1.png');

  const prisma = new PrismaClient();

  await page.getByRole('link', { name: 'Details' }).first().click();
  await expect(page).toHaveScreenshot('name-2.png');

  const name_count1 = await prisma.name.count();
  expect(name_count1).toBe(3);

  await page.getByRole('link', { name: 'Back to List' }).click();
  await page.getByRole('link', { name: 'Create New' }).click();
  await page.getByLabel('Name1').fill('bbb');
  await expect(page).toHaveScreenshot('name-3.png');
  await page.getByRole('button', { name: 'Create' }).click();
  await expect(page).toHaveScreenshot('name-4.png');

  await page.getByRole('link', { name: 'Details' }).nth(3).click();
  await expect(page).toHaveScreenshot('name-5.png');
  await page.getByText('bbb').click();

  const name_count2 = await prisma.name.count();
  expect(name_count2).toBe(4);
  const name_first = await prisma.name.findFirst();
  expect(name_first?.name).toBe('太郎');
});
