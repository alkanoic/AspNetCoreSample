import { test, expect } from '@playwright/test';
import { PrismaClient } from '@prisma/client';

test('name db create test', async ({ page }) => {
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
  const input_name = 'bbb';
  await page.getByLabel('Name1').fill(input_name);
  await expect(page).toHaveScreenshot('name-3.png');
  await page.getByRole('button', { name: 'Create' }).click();
  await expect(page).toHaveScreenshot('name-4.png');

  await page.getByRole('link', { name: 'Details' }).nth(3).click();
  await expect(page).toHaveScreenshot('name-5.png');
  await page.pause();
  await page.getByText(input_name).click();

  const url = await page.url();
  const create_id = parseInt(url.match(/(\d+)$/g)[0], 10);
  const create_name = await prisma.name.findUnique({
    where: {
      id: create_id,
    },
  });
  expect(create_name.name).toBe('bbb');

  await prisma.name.delete({
    where: {
      id: create_id,
    },
  });
});
