import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  // Run tests sequentially — backend state is shared
  fullyParallel: false,
  workers: 1,
  retries: 1,

  reporter: [
    ['html', { outputFolder: 'playwright-report', open: 'never' }],
    ['list'],
  ],

  use: {
    baseURL: 'http://localhost:4200',

    // Record video of every test
    video: 'on',

    // Capture full trace (screenshots + network + console + DOM snapshots)
    trace: 'on',

    // Screenshot on failure AND at the end of every test step
    screenshot: 'on',

    // Show browser window (not headless) so you can watch tests run
    headless: false,

    // Slow actions down slightly so they look natural on screen
    actionTimeout: 15_000,
    navigationTimeout: 30_000,

    // Default viewport
    viewport: { width: 1440, height: 900 },
  },

  projects: [
    // === Step 1: Login once, save auth state to auth.json ===
    {
      name: 'setup',
      testMatch: /global-setup\.ts/,
    },

    // === Step 2: Run all E2E tests using saved auth ===
    {
      name: 'e2e',
      testMatch: /tests\/.*\.spec\.ts/,
      dependencies: ['setup'],
      use: {
        storageState: 'auth.json',
      },
    },
  ],

  // Start the Angular dev server automatically if it isn't already running
  webServer: {
    command: 'npm run start',
    cwd: '../frontend/recruitiq-frontend',
    url: 'http://localhost:4200',
    reuseExistingServer: true,
    timeout: 120_000,
    stdout: 'ignore',
    stderr: 'pipe',
  },
});
