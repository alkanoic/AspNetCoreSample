import { test, expect } from '@playwright/test';

test('session page stores and displays session data', async ({ page }) => {
  await page.goto('/Session');
  const input = 'session-value-123';
  await page.locator('input[name="id"]').fill(input);
  await page.getByRole('button', { name: '送信' }).click();
  await expect(page.getByText(input, { exact: true })).toBeVisible();
});

test('jquery page calls sample api and loads partial view', async ({ page }) => {
  await page.goto('/JQuery');
  const input = 'jquery-input';
  await page.locator('.sample-input').fill(input);
  await page.locator('.sample-button').click();
  await expect(page.locator('.sample-label')).toContainText(input);

  await page.getByRole('button', { name: '部分ビューを読み込む' }).click();
  await expect(page.locator('#partialViewContainer')).toContainText('これは部分ビューからのデータです');
});

test('chat widget opens and sends a message', async ({ page }) => {
  await page.goto('/Chat');
  await page.locator('#chat-toggle-btn').click();
  await expect(page.locator('#chat-window')).toBeVisible();
  await page.locator('#message-input').fill('こんにちは');
  await page.locator('#chat-form button[type="submit"]').click();
  await expect(page.locator('#chat-messages .message.sent')).toContainText('こんにちは');
  await expect(page.locator('#chat-messages .message.received').last()).toContainText('ありがとうございます');
});

test('component page renders view components and buttons respond', async ({ page }) => {
  await page.goto('/Component');
  await expect(page.getByRole('heading', { name: 'Razor Component' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'View Component' })).toBeVisible();
  await page.locator('#No1').click();
  await expect(page.locator('#No1-result')).toHaveText('Button Clicked!');
});

test('htmx page renders request form', async ({ page }) => {
  await page.goto('/Htmx');
  await expect(page.locator('#request')).toBeVisible();
  await expect(page.locator('#target')).toHaveText('ここが変わる');
});

test('fluent page renders validation form', async ({ page }) => {
  await page.goto('/Fluent');
  await expect(page.locator('form')).toBeVisible();
  await expect(page.locator('#Name')).toBeVisible();
  await expect(page.locator('#Email')).toBeVisible();
});

test('bootstrap page renders contact form', async ({ page }) => {
  await page.goto('/Bootstrap');
  await expect(page.getByRole('heading', { name: 'お問い合わせフォーム' })).toBeVisible();
  await expect(page.locator('#username')).toBeVisible();
});
