/**
 * global-setup.ts
 *
 * Runs once before the entire test suite.
 * Logs in as the default admin and saves the localStorage auth token
 * to auth.json so all tests can start already authenticated.
 */
import { test as setup, expect } from '@playwright/test';

const ADMIN_EMAIL    = 'admin@recruitiq.ai';
const ADMIN_PASSWORD = 'Admin@123';
const AUTH_FILE      = 'auth.json';

setup('authenticate as admin', async ({ page }) => {
  await page.goto('/login');

  // Fill credentials
  await page.getByPlaceholder('admin@recruitiq.ai').fill(ADMIN_EMAIL);
  await page.getByPlaceholder('••••••••').fill(ADMIN_PASSWORD);
  await page.getByRole('button', { name: 'Sign In' }).click();

  // Wait until we land on dashboard
  await page.waitForURL('**/dashboard', { timeout: 15_000 });
  await expect(page.getByText('Total Candidates')).toBeVisible();

  // Persist cookies + localStorage so tests don't need to log in
  await page.context().storageState({ path: AUTH_FILE });
});
