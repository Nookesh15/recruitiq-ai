import { Page, expect } from '@playwright/test';
import path from 'path';

export interface CreateCandidateInput {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
}

export class CandidatesPage {
  constructor(private readonly page: Page) {}

  async goto() {
    await this.page.getByRole('link', { name: /candidates/i }).click();
    await this.page.waitForURL('**/candidates');
    await this.page.waitForLoadState('networkidle');
  }

  async clickAddCandidate() {
    await this.page.getByRole('button', { name: '+ Add Candidate' }).click();
    await expect(this.page.getByText('Add Candidate')).toBeVisible();
  }

  async fillCandidateForm(input: CreateCandidateInput) {
    await this.page.getByPlaceholder('Priya').fill(input.firstName);
    await this.page.getByPlaceholder('Sharma').fill(input.lastName);
    await this.page.getByPlaceholder('priya@example.com').fill(input.email);
    if (input.phone) {
      await this.page.getByPlaceholder('+91 9999999999').fill(input.phone);
    }
  }

  async submitCandidateForm() {
    await this.page.getByRole('button', { name: 'Add Candidate' }).click();
  }

  async createCandidate(input: CreateCandidateInput) {
    await this.clickAddCandidate();
    await this.fillCandidateForm(input);
    await this.submitCandidateForm();
    await expect(this.page.getByText('Add Candidate')).not.toBeVisible({ timeout: 10_000 });
    await expect(
      this.page.getByText(`${input.firstName} ${input.lastName}`)
    ).toBeVisible();
  }

  async search(query: string) {
    await this.page.getByPlaceholder('Search by name or email...').fill(query);
    // Debounce — small wait
    await this.page.waitForTimeout(400);
  }

  async clearSearch() {
    await this.page.getByPlaceholder('Search by name or email...').fill('');
    await this.page.waitForTimeout(300);
  }

  async clickCandidate(name: string) {
    await this.page.getByText(name).click();
    await this.page.waitForURL('**/candidates/**');
    await this.page.waitForLoadState('networkidle');
  }

  async expectCandidateInList(name: string) {
    await expect(this.page.getByText(name)).toBeVisible();
  }

  async expectCandidateCount(count: number) {
    await expect(this.page.getByText(`${count} candidates found`)).toBeVisible();
  }

  async expectEmptySearch(query: string) {
    await expect(this.page.getByText(`No candidates matching "${query}"`)).toBeVisible();
  }

  async expectValidationError(msg: string) {
    await expect(this.page.getByText(msg)).toBeVisible();
  }
}

export class CandidateDetailPage {
  constructor(private readonly page: Page) {}

  async expectName(name: string) {
    await expect(this.page.getByRole('heading', { name })).toBeVisible();
  }

  async changeStatus(status: string) {
    await this.page.locator('select').first().selectOption(status);
    await this.page.waitForTimeout(500);
    await expect(this.page.getByText(status)).toBeVisible();
  }

  async uploadResume(filePath: string) {
    await this.page.locator('input[type="file"]').setInputFiles(filePath);
    await this.page.getByRole('button', { name: /upload.*score/i }).click();
  }

  async clickApplyToJob() {
    await this.page.getByRole('button', { name: 'Apply to Job' }).click();
    await expect(this.page.getByText(/select a job/i)).toBeVisible();
  }

  async goBack() {
    await this.page.getByRole('link', { name: /back to candidates/i }).click();
    await this.page.waitForURL('**/candidates');
  }
}
