import { Page, expect } from '@playwright/test';

export class DashboardPage {
  constructor(private readonly page: Page) {}

  async goto() {
    await this.page.goto('/dashboard');
    await this.page.waitForLoadState('networkidle');
  }

  async expectStatCards() {
    await expect(this.page.getByText('Total Candidates')).toBeVisible();
    await expect(this.page.getByText('Active Jobs')).toBeVisible();
    await expect(this.page.getByText('Total Applications')).toBeVisible();
    await expect(this.page.getByText('Avg AI Score')).toBeVisible();
  }

  async expectRecentApplicationsSection() {
    await expect(this.page.getByText('Recent Applications')).toBeVisible();
  }

  async expectFunnelSection() {
    await expect(this.page.getByText('Hiring Funnel').or(this.page.getByText('Funnel'))).toBeVisible();
  }

  async clickViewAllCandidates() {
    await this.page.getByRole('link', { name: /view all/i }).first().click();
    await this.page.waitForURL('**/candidates');
  }

  /** Navigate using the sidebar */
  async navigateTo(label: string) {
    await this.page.getByRole('link', { name: label }).click();
  }
}
