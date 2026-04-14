/**
 * 01 — Authentication flow
 *
 * Tests login page validation, wrong-password error, and successful login.
 * These tests do NOT use the saved auth state — they start unauthenticated.
 */
import { test, expect } from '@playwright/test';
import { LoginPage } from '../pages/login.page';

// Override the project-level storageState for auth tests
test.use({ storageState: { cookies: [], origins: [] } });

test.describe('Authentication', () => {
  test('shows validation errors when submitting empty form', async ({ page }) => {
    const login = new LoginPage(page);

    await test.step('Open login page', async () => {
      await login.goto();
      await expect(page.getByText('Welcome to RecruitIQ')).toBeVisible();
    });

    await test.step('Click Sign In without filling anything', async () => {
      await login.clickSignIn();
    });

    await test.step('Both fields show required errors', async () => {
      await expect(page.getByText('Email is required.')).toBeVisible();
      await expect(page.getByText('Password is required.')).toBeVisible();
    });
  });

  test('shows error for invalid email format', async ({ page }) => {
    const login = new LoginPage(page);
    await login.goto();

    await test.step('Type a non-email value and tab away', async () => {
      await login.fillEmail('not-an-email');
      await login.fillPassword('x');
      await login.clickSignIn();
    });

    await test.step('Email format error appears', async () => {
      await expect(page.getByText('Enter a valid email address.')).toBeVisible();
    });
  });

  test('shows error for short password', async ({ page }) => {
    const login = new LoginPage(page);
    await login.goto();

    await test.step('Type valid email but short password', async () => {
      await login.fillEmail('admin@recruitiq.ai');
      await login.fillPassword('abc');
      await login.clickSignIn();
    });

    await test.step('Min-length error appears', async () => {
      await expect(page.getByText('Password must be at least 6 characters.')).toBeVisible();
    });
  });

  test('shows API error for wrong credentials', async ({ page }) => {
    const login = new LoginPage(page);

    await test.step('Open login page', async () => {
      await login.goto();
    });

    await test.step('Submit with wrong password', async () => {
      await login.fillEmail('admin@recruitiq.ai');
      await login.fillPassword('WrongPassword1');
      await login.clickSignIn();
    });

    await test.step('Error banner is shown', async () => {
      await expect(page.getByText('Invalid email or password.')).toBeVisible({ timeout: 8_000 });
    });
  });

  test('successful login redirects to dashboard', async ({ page }) => {
    const login = new LoginPage(page);

    await test.step('Go to login page', async () => {
      await login.goto();
    });

    await test.step('Fill valid admin credentials', async () => {
      await login.fillEmail('admin@recruitiq.ai');
      await login.fillPassword('Admin@123');
    });

    await test.step('Click Sign In', async () => {
      await login.clickSignIn();
    });

    await test.step('Lands on dashboard and stat cards are visible', async () => {
      await page.waitForURL('**/dashboard', { timeout: 15_000 });
      await expect(page.getByText('Total Candidates')).toBeVisible();
      await expect(page.getByText('Active Jobs')).toBeVisible();
    });

    await test.step('Sidebar shows user name', async () => {
      await expect(page.getByText('Admin')).toBeVisible();
    });
  });

  test('logout clears session and returns to login', async ({ page }) => {
    const login = new LoginPage(page);

    // Login first
    await login.goto();
    await login.fillEmail('admin@recruitiq.ai');
    await login.fillPassword('Admin@123');
    await login.clickSignIn();
    await page.waitForURL('**/dashboard', { timeout: 15_000 });

    await test.step('Click Sign out in sidebar', async () => {
      await page.getByRole('button', { name: /sign out/i }).click();
    });

    await test.step('Redirected to /login', async () => {
      await page.waitForURL('**/login', { timeout: 8_000 });
      await expect(page.getByText('Welcome to RecruitIQ')).toBeVisible();
    });

    await test.step('Protected route redirects back to login', async () => {
      await page.goto('/dashboard');
      await page.waitForURL('**/login');
    });
  });
});
