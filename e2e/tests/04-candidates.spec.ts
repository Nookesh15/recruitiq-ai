/**
 * 04 — Candidates
 *
 * Full candidate lifecycle:
 *   • form validation
 *   • create candidate
 *   • search / filter
 *   • navigate to detail page
 *   • change pipeline stage on detail
 *   • apply candidate to a job
 */
import { test, expect } from '@playwright/test';
import { CandidatesPage, CandidateDetailPage } from '../pages/candidates.page';
import path from 'path';

const CANDIDATE = {
  firstName: 'E2E',
  lastName: `Tester${Date.now()}`,
  email: `e2e.tester${Date.now()}@example.com`,
  phone: '+91 9876543210',
};
const FULL_NAME = `${CANDIDATE.firstName} ${CANDIDATE.lastName}`;

test.describe('Candidates', () => {
  test('navigates to candidates page', async ({ page }) => {
    await page.goto('/dashboard');

    await test.step('Click Candidates in sidebar', async () => {
      await page.getByRole('link', { name: /^candidates$/i }).click();
      await page.waitForURL('**/candidates');
    });

    await test.step('Candidates page visible', async () => {
      await expect(page.getByText('Candidates')).toBeVisible();
    });
  });

  test('shows validation errors on empty candidate form', async ({ page }) => {
    const candidates = new CandidatesPage(page);
    await page.goto('/candidates');
    await page.waitForLoadState('networkidle');

    await test.step('Open Add Candidate modal', async () => {
      await candidates.clickAddCandidate();
    });

    await test.step('Submit without filling anything', async () => {
      await page.getByRole('button', { name: 'Add Candidate' }).click();
    });

    await test.step('Required errors visible', async () => {
      await expect(page.getByText('First name is required.')).toBeVisible();
      await expect(page.getByText('Last name is required.')).toBeVisible();
      await expect(page.getByText('Email is required.')).toBeVisible();
    });

    await test.step('Cancel closes modal', async () => {
      await page.getByRole('button', { name: 'Cancel' }).click();
      await expect(page.getByText('Add Candidate')).not.toBeVisible();
    });
  });

  test('shows invalid-email error', async ({ page }) => {
    const candidates = new CandidatesPage(page);
    await page.goto('/candidates');
    await page.waitForLoadState('networkidle');

    await candidates.clickAddCandidate();

    await test.step('Fill in bad email', async () => {
      await page.getByPlaceholder('Priya').fill('Jane');
      await page.getByPlaceholder('Sharma').fill('Doe');
      await page.getByPlaceholder('priya@example.com').fill('not-an-email');
      await page.getByRole('button', { name: 'Add Candidate' }).click();
    });

    await test.step('Email error shown', async () => {
      await expect(page.getByText('Enter a valid email address.')).toBeVisible();
    });

    await page.getByRole('button', { name: 'Cancel' }).click();
  });

  test('creates a new candidate end-to-end', async ({ page }) => {
    const candidates = new CandidatesPage(page);
    await page.goto('/candidates');
    await page.waitForLoadState('networkidle');

    await test.step('Open modal', async () => {
      await candidates.clickAddCandidate();
    });

    await test.step('Fill all fields', async () => {
      await candidates.fillCandidateForm(CANDIDATE);
    });

    await test.step('Submit and candidate appears in table', async () => {
      await candidates.submitCandidateForm();
      await expect(page.getByText('Add Candidate')).not.toBeVisible({ timeout: 10_000 });
      await expect(page.getByText(FULL_NAME)).toBeVisible();
    });

    await test.step('Candidate shows Applied status badge', async () => {
      const row = page.locator('tr', { hasText: CANDIDATE.email });
      await expect(row.getByText('Applied')).toBeVisible();
    });
  });

  test('search filters candidates by name', async ({ page }) => {
    const candidates = new CandidatesPage(page);
    await page.goto('/candidates');
    await page.waitForLoadState('networkidle');

    // Ensure at least our candidate exists
    const exists = await page.getByText(FULL_NAME).isVisible().catch(() => false);
    if (!exists) {
      await candidates.createCandidate(CANDIDATE);
    }

    await test.step('Search by first name', async () => {
      await candidates.search(CANDIDATE.firstName);
    });

    await test.step('Only matching candidates shown', async () => {
      await expect(page.getByText(FULL_NAME)).toBeVisible();
    });

    await test.step('Search by non-existent name shows empty state', async () => {
      await candidates.search('zzznomatch');
      await candidates.expectEmptySearch('zzznomatch');
    });

    await test.step('Clear search restores full list', async () => {
      await candidates.clearSearch();
      await expect(page.getByText(FULL_NAME)).toBeVisible();
    });
  });

  test('navigates to candidate detail page', async ({ page }) => {
    const candidates = new CandidatesPage(page);
    const detail = new CandidateDetailPage(page);
    await page.goto('/candidates');
    await page.waitForLoadState('networkidle');

    // Ensure our candidate exists
    const exists = await page.getByText(FULL_NAME).isVisible().catch(() => false);
    if (!exists) {
      await candidates.createCandidate(CANDIDATE);
    }

    await test.step('Click on candidate row', async () => {
      await page.getByText(FULL_NAME).first().click();
      await page.waitForURL('**/candidates/**');
    });

    await test.step('Detail page shows candidate name', async () => {
      await expect(page.getByText(FULL_NAME)).toBeVisible();
    });

    await test.step('Email and phone visible on profile card', async () => {
      await expect(page.getByText(CANDIDATE.email)).toBeVisible();
      await expect(page.getByText(CANDIDATE.phone!)).toBeVisible();
    });

    await test.step('Status select present', async () => {
      await expect(page.locator('select').first()).toBeVisible();
    });
  });

  test('changes pipeline stage on detail page', async ({ page }) => {
    const candidates = new CandidatesPage(page);
    const detail = new CandidateDetailPage(page);
    await page.goto('/candidates');
    await page.waitForLoadState('networkidle');

    // Ensure our candidate exists
    const exists = await page.getByText(FULL_NAME).isVisible().catch(() => false);
    if (!exists) {
      await candidates.createCandidate(CANDIDATE);
    }

    await test.step('Open candidate detail', async () => {
      await page.getByText(FULL_NAME).first().click();
      await page.waitForURL('**/candidates/**');
      await page.waitForLoadState('networkidle');
    });

    await test.step('Change status to Screening', async () => {
      await detail.changeStatus('Screening');
    });

    await test.step('Status badge updated', async () => {
      await expect(page.getByText('Screening').first()).toBeVisible();
    });

    await test.step('Change status to Interview', async () => {
      await detail.changeStatus('Interview');
      await expect(page.getByText('Interview').first()).toBeVisible();
    });

    await test.step('Go back to candidates list', async () => {
      await detail.goBack();
      await page.waitForURL('**/candidates');
    });
  });

  test('resume upload button is present on detail page', async ({ page }) => {
    const candidates = new CandidatesPage(page);
    await page.goto('/candidates');
    await page.waitForLoadState('networkidle');

    const exists = await page.getByText(FULL_NAME).isVisible().catch(() => false);
    if (!exists) {
      await candidates.createCandidate(CANDIDATE);
    }

    await page.getByText(FULL_NAME).first().click();
    await page.waitForURL('**/candidates/**');

    await test.step('File input and Upload & Score button visible', async () => {
      await expect(page.locator('input[type="file"]')).toBeVisible();
      await expect(page.getByRole('button', { name: /upload.*score/i })).toBeVisible();
    });
  });
});
