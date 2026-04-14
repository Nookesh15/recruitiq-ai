import { Page, expect } from '@playwright/test';

export interface CreateUserInput {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: 'Recruiter' | 'Admin';
}

export class UsersPage {
  constructor(private readonly page: Page) {}

  async goto() {
    await this.page.getByRole('link', { name: /users/i }).click();
    await this.page.waitForURL('**/users');
    await this.page.waitForLoadState('networkidle');
  }

  async expectHeading() {
    await expect(this.page.getByText('User Management')).toBeVisible();
  }

  async clickAddUser() {
    await this.page.getByRole('button', { name: '+ Add User' }).click();
    await expect(this.page.getByText('New User')).toBeVisible();
  }

  async fillUserForm(input: CreateUserInput) {
    await this.page.getByPlaceholder('First name').fill(input.firstName);
    await this.page.getByPlaceholder('Last name').fill(input.lastName);
    await this.page.getByPlaceholder('Email').fill(input.email);
    await this.page.getByPlaceholder('Password').fill(input.password);
    await this.page.locator('select').selectOption(input.role);
  }

  async submitUserForm() {
    await this.page.getByRole('button', { name: 'Create User' }).click();
  }

  async createUser(input: CreateUserInput) {
    await this.clickAddUser();
    await this.fillUserForm(input);
    await this.submitUserForm();
    // Wait for form to collapse
    await expect(this.page.getByText('New User')).not.toBeVisible({ timeout: 10_000 });
    await expect(
      this.page.getByText(`${input.firstName} ${input.lastName}`)
    ).toBeVisible();
  }

  async expectUserInTable(name: string, role: string) {
    await expect(this.page.getByText(name)).toBeVisible();
    await expect(this.page.getByText(role).first()).toBeVisible();
  }
}
