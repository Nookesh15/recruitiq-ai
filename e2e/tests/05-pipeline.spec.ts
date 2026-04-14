/**
 * 05 — Pipeline (Kanban Board)
 *
 * Verifies the kanban view loads and shows all stage columns.
 */
import { test, expect } from '@playwright/test';

test.describe('Pipeline (Kanban)', () => {
  test('navigates to pipeline page', async ({ page }) => {
    await page.goto('/dashboard');

    await test.step('Click Pipeline in sidebar', async () => {
      await page.getByRole('link', { name: /pipeline/i }).click();
      await page.waitForURL('**/pipeline');
    });

    await test.step('Pipeline page loads', async () => {
      await expect(page).toHaveURL(/\/pipeline/);
    });
  });

  test('shows all pipeline stage columns', async ({ page }) => {
    await page.goto('/pipeline');
    await page.waitForLoadState('networkidle');

    await test.step('All stage columns visible', async () => {
      await expect(page.getByText('Applied')).toBeVisible();
      await expect(page.getByText('Screening')).toBeVisible();
      await expect(page.getByText('Interview')).toBeVisible();
      await expect(page.getByText('Offer')).toBeVisible();
      await expect(page.getByText('Hired')).toBeVisible();
    });
  });
});
