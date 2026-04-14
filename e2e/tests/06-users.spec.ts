/**
 * 06 — User Management (Admin only)
 *
 * Verifies admin can view users list and create a new recruiter account.
 */
import { test, expect } from '@playwright/test';
import { UsersPage } from '../pages/users.page';

const NEW_USER = {
  firstName: 'E2E',
  lastName: `Recruiter${Date.now()}`,
  email: `e2e.recruiter${Date.now()}@example.com`,
  password: 'Recruiter@123',
  role: 'Recruiter' as const,
};

test.describe('User Management (Admin)', () => {
  test('navigates to users page', async ({ page }) => {
    await page.goto('/dashboard');

    await test.step('Click Users in sidebar', async () => {
      await page.getByRole('link', { name: /users/i }).click();
      await page.waitForURL('**/users');
    });

    await test.step('User Management heading visible', async () => {
      await expect(page.getByText('User Management')).toBeVisible();
      await expect(page.getByText('Admin-only')).toBeVisible();
    });
  });

  test('admin user appears in the users table', async ({ page }) => {
    await page.goto('/users');
    await page.waitForLoadState('networkidle');

    await test.step('Admin row is in the table', async () => {
      await expect(page.getByText('admin@recruitiq.ai')).toBeVisible();
    });

    await test.step('Admin role badge shows', async () => {
      const adminRow = page.locator('tr', { hasText: 'admin@recruitiq.ai' });
      await expect(adminRow.getByText('Admin')).toBeVisible();
    });
  });

  test('creates a new recruiter user', async ({ page }) => {
    const users = new UsersPage(page);
    await page.goto('/users');
    await page.waitForLoadState('networkidle');

    await test.step('Click + Add User', async () => {
      await users.clickAddUser();
    });

    await test.step('Fill new user form', async () => {
      await users.fillUserForm(NEW_USER);
    });

    await test.step('Submit and new user appears in table', async () => {
      await users.submitUserForm();
      const fullName = `${NEW_USER.firstName} ${NEW_USER.lastName}`;
      await expect(page.getByText(fullName)).toBeVisible({ timeout: 10_000 });
    });

    await test.step('New user has Recruiter role badge', async () => {
      const row = page.locator('tr', { hasText: NEW_USER.email });
      await expect(row.getByText('Recruiter')).toBeVisible();
    });
  });

  test('newly created user can log in', async ({ page, context }) => {
    // Create a uniquely-named user for this test
    const ts = Date.now();
    const testUser = {
      firstName: 'Login',
      lastName: `Test${ts}`,
      email: `logintest${ts}@example.com`,
      password: 'LoginTest@123',
      role: 'Recruiter' as const,
    };

    await test.step('Create the user via admin panel', async () => {
      await page.goto('/users');
      await page.waitForLoadState('networkidle');
      const users = new UsersPage(page);
      await users.createUser(testUser);
    });

    await test.step('Open a fresh browser context and log in as new user', async () => {
      const newPage = await context.newPage();
      await newPage.goto('/login');
      await newPage.getByPlaceholder('admin@recruitiq.ai').fill(testUser.email);
      await newPage.getByPlaceholder('••••••••').fill(testUser.password);
      await newPage.getByRole('button', { name: 'Sign In' }).click();
      await newPage.waitForURL('**/dashboard', { timeout: 15_000 });
      await expect(newPage.getByText('Total Candidates')).toBeVisible();
      await newPage.close();
    });
  });
});
