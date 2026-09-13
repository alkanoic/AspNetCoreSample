import { test, expect } from '@playwright/test';

const publicPages: { name: string; path: string }[] = [
  { name: 'Name', path: '/Name' },
  { name: 'Map', path: '/Map' },
  { name: 'Fluent', path: '/Fluent' },
  { name: 'Bootstrap', path: '/Bootstrap' },
  { name: 'Vue', path: '/Vue' },
  { name: 'Vue Component', path: '/VueComponent' },
  { name: 'Component', path: '/Component' },
  { name: 'jQuery', path: '/JQuery' },
  { name: 'Lit', path: '/Lit' },
  { name: 'Htmx', path: '/Htmx' },
  { name: 'Vite', path: '/Vite' },
  { name: 'Chat', path: '/Chat' },
  { name: 'Session', path: '/Session' },
  { name: 'Qr', path: '/QrCode' },
  { name: 'QrNotification', path: '/QrCodeNotification' },
];

for (const pageInfo of publicPages) {
  test(`navigate to ${pageInfo.name} page`, async ({ page }) => {
    await page.goto('/');
    const viewportSize = page.viewportSize();
    if (viewportSize.width <= 480) {
      await page.getByLabel('Toggle navigation').click();
    }
    await page.getByRole('link', { name: pageInfo.name, exact: true }).click();
    await expect(page).toHaveURL(new RegExp(`${pageInfo.path}$`));
  });
}
