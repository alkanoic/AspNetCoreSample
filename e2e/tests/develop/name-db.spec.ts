import { test, expect } from '@playwright/test';
import { PrismaClient } from '@prisma/client';

test('name db create test', async ({ page }, testinfo) => {
  testinfo.config.workers = 1;
  await page.goto('/');
  const viewportSize = page.viewportSize();
  if (viewportSize.width <= 480) {
    // モバイルデバイスのコードを実行
    await page.getByLabel('Toggle navigation').click();
  }
  await page.getByRole('link', { name: 'Name' }).click();
  await expect(page).toHaveScreenshot('name-1.png');

  await page.getByRole('link', { name: 'Details' }).first().click();
  await expect(page).toHaveScreenshot('name-2.png');

  await page.getByRole('link', { name: 'Back to List' }).click();
  await page.getByRole('link', { name: 'Create New' }).click();
  const input_name = 'bbb';
  await page.getByLabel('Name1').fill(input_name);
  await expect(page).toHaveScreenshot('name-3.png');
  await page.getByRole('button', { name: 'Create' }).click();
  await expect(page).toHaveScreenshot('name-4.png');

  await page.getByRole('link', { name: 'Details' }).nth(3).click();
  await expect(page).toHaveScreenshot('name-5.png');
  await page.getByText(input_name).click();

  const url = await page.url();
  const create_id = parseInt(url.match(/(\d+)$/g)[0], 10);
  const prisma = new PrismaClient();
  const create_name = await prisma.name.findUnique({
    where: {
      id: create_id,
    },
  });
  expect(create_name.name).toBe(input_name);

  await page.getByRole('link', { name: 'Back to List' }).click();
  await page.getByRole('link', { name: 'Delete' }).nth(3).click();
  await page.getByRole('button', { name: 'Delete' }).click();
  await expect(page).toHaveScreenshot('name-6.png');

  const delete_name = await prisma.name.findUnique({
    where: {
      id: create_id,
    },
  });
  expect(delete_name).toBeNull();
});
