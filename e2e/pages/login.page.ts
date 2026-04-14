import { Page, expect } from '@playwright/test';

export class LoginPage {
  constructor(private readonly page: Page) {}

  async goto() {
    await this.page.goto('/login');
  }

  async fillEmail(value: string) {
    await this.page.getByPlaceholder('admin@recruitiq.ai').fill(value);
  }

  async fillPassword(value: string) {
    await this.page.getByPlaceholder('••••••••').fill(value);
  }

  async clickSignIn() {
    await this.page.getByRole('button', { name: 'Sign In' }).click();
  }

  async login(email: string, password: string) {
    await this.goto();
    await this.fillEmail(email);
    await this.fillPassword(password);
    await this.clickSignIn();
  }

  async expectEmailError(msg: string) {
    await expect(this.page.getByText(msg)).toBeVisible();
  }

  async expectApiError(msg: string) {
    await expect(this.page.getByText(msg)).toBeVisible();
  }
}
