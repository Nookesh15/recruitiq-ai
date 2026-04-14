#!/usr/bin/env bash
# ─────────────────────────────────────────────────────────────
# RecruitIQ — E2E Test Runner
#
# Usage:
#   ./run-e2e.sh              # install + run all tests (headed, with recording)
#   ./run-e2e.sh --ui         # open Playwright interactive UI
#   ./run-e2e.sh --report     # just open the last HTML report
#   ./run-e2e.sh --headless   # run headless (no browser window)
# ─────────────────────────────────────────────────────────────
set -e

E2E_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$E2E_DIR"

BACKEND_URL="http://localhost:5000/api/v1/health"
FRONTEND_URL="http://localhost:4200"

# ── helpers ──────────────────────────────────────────────────
bold()  { printf '\033[1m%s\033[0m\n' "$*"; }
green() { printf '\033[32m%s\033[0m\n' "$*"; }
yellow(){ printf '\033[33m%s\033[0m\n' "$*"; }
red()   { printf '\033[31m%s\033[0m\n' "$*"; }

# ── pre-flight: check backend ─────────────────────────────────
bold "1. Checking backend ($BACKEND_URL)…"
if curl -sf "$BACKEND_URL" -o /dev/null 2>/dev/null; then
  green "   ✓ Backend is running."
else
  red   "   ✗ Backend is NOT running at $BACKEND_URL"
  echo  ""
  echo  "   Start the backend first:"
  echo  "     cd ../backend && dotnet run --project RecruitIQ.API"
  echo  ""
  echo  "   Or start with Docker Compose:"
  echo  "     docker compose -f ../infrastructure/docker/docker-compose.prod.yml up -d"
  echo  ""
  exit 1
fi

# ── install Playwright if needed ──────────────────────────────
bold "2. Installing/verifying Playwright…"
if [ ! -d node_modules ]; then
  npm install
fi
# Install browsers (idempotent)
npx playwright install chromium --with-deps 2>&1 | tail -5

# ── run tests ────────────────────────────────────────────────
bold "3. Running E2E tests…"
echo  ""

if [ "$1" = "--ui" ]; then
  yellow "Opening Playwright UI (interactive mode)…"
  npx playwright test --ui
elif [ "$1" = "--headless" ]; then
  npx playwright test --headed=false
elif [ "$1" = "--report" ]; then
  npx playwright show-report playwright-report
else
  # Default: headless:false is set in playwright.config.ts — browser window visible
  npx playwright test
fi

EXIT_CODE=$?

# ── show report ───────────────────────────────────────────────
bold "4. Opening HTML report…"
npx playwright show-report playwright-report

exit $EXIT_CODE
