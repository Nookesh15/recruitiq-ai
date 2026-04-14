import { Page, expect } from '@playwright/test';

export interface CreateJobInput {
  title: string;
  department: string;   // option value, e.g. 'Engineering'
  location: string;
  description: string;  // must be 50+ chars
}

export class JobsPage {
  constructor(private readonly page: Page) {}

  async goto() {
    await this.page.getByRole('link', { name: /job postings/i }).click();
    await this.page.waitForURL('**/jobs');
    await this.page.waitForLoadState('networkidle');
  }

  async clickNewJob() {
    await this.page.getByRole('button', { name: '+ New Job' }).click();
    await expect(this.page.getByText('Create Job Posting')).toBeVisible();
  }

  async fillJobForm(input: CreateJobInput) {
    await this.page.getByPlaceholder('Senior Angular Developer').fill(input.title);

    // Department select — pick by visible option text
    await this.page.locator('select[formControlName="department"]').selectOption({ label: input.department });

    await this.page.getByPlaceholder('Hyderabad / Remote').fill(input.location);
    await this.page.locator('textarea[formControlName="description"]').fill(input.description);
  }

  async submitJobForm() {
    await this.page.getByRole('button', { name: 'Create Job' }).click();
  }

  async createJob(input: CreateJobInput) {
    await this.clickNewJob();
    await this.fillJobForm(input);
    await this.submitJobForm();
    // Wait for modal to close and card to appear
    await expect(this.page.getByText('Create Job Posting')).not.toBeVisible({ timeout: 10_000 });
    await expect(this.page.getByText(input.title)).toBeVisible();
  }

  /** Change the status of a job card via its dropdown */
  async changeJobStatus(jobTitle: string, newStatus: string) {
    const card = this.page.locator('.bg-white.rounded-xl', { hasText: jobTitle }).first();
    await card.locator('select').selectOption(newStatus);
    // Wait for the status badge to update
    await expect(card.getByText(newStatus)).toBeVisible({ timeout: 8_000 });
  }

  /** Click "View Applicants" on a job card */
  async viewApplicants(jobTitle: string) {
    const card = this.page.locator('.bg-white.rounded-xl', { hasText: jobTitle }).first();
    await card.getByRole('button', { name: /view applicants/i }).click();
    await this.page.waitForURL('**/applicants');
  }

  /** Click "Copy Link" on a job card */
  async copyApplyLink(jobTitle: string) {
    const card = this.page.locator('.bg-white.rounded-xl', { hasText: jobTitle }).first();
    await card.getByRole('button', { name: /copy link/i }).click();
  }

  async expectEmptyState() {
    await expect(this.page.getByText(/no job postings yet/i)).toBeVisible();
  }

  async expectValidationError(msg: string) {
    await expect(this.page.getByText(msg)).toBeVisible();
  }
}
