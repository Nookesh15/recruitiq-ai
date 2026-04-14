/**
 * 02 — Dashboard
 *
 * Verifies all stat cards, recent applications panel, and funnel chart
 * are rendered after login.
 */
import { test, expect } from '@playwright/test';
import { DashboardPage } from '../pages/dashboard.page';

test.describe('Dashboard', () => {
  test('loads all stat cards', async ({ page }) => {
    const dashboard = new DashboardPage(page);

    await test.step('Navigate to dashboard', async () => {
      await dashboard.goto();
    });

    await test.step('All four stat cards are visible', async () => {
      await dashboard.expectStatCards();
    });
  });

  test('shows Recent Applications panel', async ({ page }) => {
    const dashboard = new DashboardPage(page);

    await test.step('Navigate to dashboard', async () => {
      await dashboard.goto();
    });

    await test.step('Recent Applications heading visible', async () => {
      await dashboard.expectRecentApplicationsSection();
    });
  });

  test('sidebar navigation links are present', async ({ page }) => {
    const dashboard = new DashboardPage(page);
    await dashboard.goto();

    await test.step('All main nav links visible', async () => {
      await expect(page.getByRole('link', { name: /dashboard/i })).toBeVisible();
      await expect(page.getByRole('link', { name: /candidates/i })).toBeVisible();
      await expect(page.getByRole('link', { name: /job postings/i })).toBeVisible();
      await expect(page.getByRole('link', { name: /pipeline/i })).toBeVisible();
      await expect(page.getByRole('link', { name: /analytics/i })).toBeVisible();
    });

    await test.step('Admin-only Users link visible for admin', async () => {
      await expect(page.getByRole('link', { name: /users/i })).toBeVisible();
    });
  });

  test('"View all" link navigates to candidates', async ({ page }) => {
    const dashboard = new DashboardPage(page);
    await dashboard.goto();

    await test.step('Click View all → candidates', async () => {
      await dashboard.clickViewAllCandidates();
    });

    await test.step('Candidates page loaded', async () => {
      await expect(page.getByText('Candidates')).toBeVisible();
    });
  });

  test('system online badge is shown', async ({ page }) => {
    const dashboard = new DashboardPage(page);
    await dashboard.goto();

    await expect(page.getByText('System Online')).toBeVisible();
  });
});
