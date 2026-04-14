/**
 * 03 — Job Postings
 *
 * Full CRUD flow:
 *   • form validation on empty submit
 *   • create a job posting
 *   • change its status
 *   • view its applicants page
 *   • copy the apply link
 */
import { test, expect } from '@playwright/test';
import { JobsPage } from '../pages/jobs.page';

const JOB = {
  title: `E2E Senior Developer ${Date.now()}`,
  department: 'Engineering',
  location: 'Hyderabad / Remote',
  description:
    'Looking for a Senior Developer with 5+ years of experience in TypeScript, Angular, and .NET. ' +
    'You will lead a small team and work closely with product managers to ship features.',
};

test.describe('Job Postings', () => {
  test('navigates to jobs page', async ({ page }) => {
    const jobs = new JobsPage(page);

    await test.step('Click Job Postings in sidebar', async () => {
      await page.goto('/dashboard');
      await jobs.goto();
    });

    await test.step('Jobs heading visible', async () => {
      await expect(page.getByText('Job Postings')).toBeVisible();
    });
  });

  test('shows validation errors when creating job with empty form', async ({ page }) => {
    const jobs = new JobsPage(page);
    await page.goto('/jobs');
    await page.waitForLoadState('networkidle');

    await test.step('Open create modal', async () => {
      await jobs.clickNewJob();
    });

    await test.step('Submit without filling anything', async () => {
      await page.getByRole('button', { name: 'Create Job' }).click();
    });

    await test.step('All required field errors shown', async () => {
      await expect(page.getByText('Job title is required')).toBeVisible();
      await expect(page.getByText('Department is required.')).toBeVisible();
      await expect(page.getByText('Location is required.')).toBeVisible();
      await expect(page.getByText('Description is required.')).toBeVisible();
    });

    await test.step('Cancel closes the modal', async () => {
      await page.getByRole('button', { name: 'Cancel' }).click();
      await expect(page.getByText('Create Job Posting')).not.toBeVisible();
    });
  });

  test('shows min-length error for short description', async ({ page }) => {
    const jobs = new JobsPage(page);
    await page.goto('/jobs');
    await page.waitForLoadState('networkidle');

    await jobs.clickNewJob();

    await test.step('Fill title and short description (< 50 chars)', async () => {
      await page.getByPlaceholder('Senior Angular Developer').fill('My Job');
      await page.locator('textarea[formControlName="description"]').fill('Too short');
    });

    await test.step('Touch description and verify min-length error', async () => {
      await page.getByRole('button', { name: 'Create Job' }).click();
      await expect(page.getByText('Description must be at least 50 characters.')).toBeVisible();
    });

    await page.getByRole('button', { name: 'Cancel' }).click();
  });

  test('creates a new job posting end-to-end', async ({ page }) => {
    const jobs = new JobsPage(page);
    await page.goto('/jobs');
    await page.waitForLoadState('networkidle');

    await test.step('Open create job modal', async () => {
      await jobs.clickNewJob();
    });

    await test.step('Fill in all required fields', async () => {
      await jobs.fillJobForm(JOB);
    });

    await test.step('Submit and verify card appears in grid', async () => {
      await jobs.submitJobForm();
      await expect(page.getByText('Create Job Posting')).not.toBeVisible({ timeout: 10_000 });
      await expect(page.getByText(JOB.title)).toBeVisible();
    });

    await test.step('New card shows Draft status', async () => {
      const card = page.locator('.bg-white.rounded-xl', { hasText: JOB.title }).first();
      await expect(card.getByText('Draft')).toBeVisible();
    });
  });

  test('updates job status via dropdown', async ({ page }) => {
    const jobs = new JobsPage(page);
    await page.goto('/jobs');
    await page.waitForLoadState('networkidle');

    // If no jobs exist yet, create one
    const cardCount = await page.locator('.bg-white.rounded-xl').count();
    if (cardCount < 2) {
      // create a fresh one (skip empty-state card)
      await jobs.createJob(JOB);
    }

    await test.step('Change status to Active', async () => {
      await jobs.changeJobStatus(JOB.title, 'Active');
    });

    await test.step('Status badge now shows Active (green)', async () => {
      const card = page.locator('.bg-white.rounded-xl', { hasText: JOB.title }).first();
      await expect(card.getByText('Active')).toBeVisible();
    });

    await test.step('Change status back to Paused', async () => {
      await jobs.changeJobStatus(JOB.title, 'Paused');
      const card = page.locator('.bg-white.rounded-xl', { hasText: JOB.title }).first();
      await expect(card.getByText('Paused')).toBeVisible();
    });
  });

  test('navigates to applicants page', async ({ page }) => {
    const jobs = new JobsPage(page);
    await page.goto('/jobs');
    await page.waitForLoadState('networkidle');

    // Ensure at least one job exists
    const hasJob = await page.getByText(JOB.title).isVisible().catch(() => false);
    if (!hasJob) {
      await jobs.createJob(JOB);
    }

    await test.step('Click View Applicants', async () => {
      await jobs.viewApplicants(JOB.title);
    });

    await test.step('Applicants page loaded', async () => {
      await expect(page).toHaveURL(/\/jobs\/.*\/applicants/);
      await expect(page.getByText(/applicants/i)).toBeVisible();
    });
  });

  test('copies apply link to clipboard', async ({ page, context }) => {
    // Grant clipboard permissions
    await context.grantPermissions(['clipboard-read', 'clipboard-write']);

    const jobs = new JobsPage(page);
    await page.goto('/jobs');
    await page.waitForLoadState('networkidle');

    const hasJob = await page.getByText(JOB.title).isVisible().catch(() => false);
    if (!hasJob) {
      await jobs.createJob(JOB);
    }

    await test.step('Click Copy Link', async () => {
      await jobs.copyApplyLink(JOB.title);
    });

    await test.step('Clipboard contains /apply/ URL', async () => {
      const text = await page.evaluate(() => navigator.clipboard.readText());
      expect(text).toContain('/apply/');
    });
  });
});
