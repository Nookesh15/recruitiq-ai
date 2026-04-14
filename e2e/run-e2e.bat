@echo off
REM ─────────────────────────────────────────────────────────────
REM RecruitIQ — E2E Test Runner (Windows)
REM
REM Usage:
REM   run-e2e.bat           — install + run all tests (headed, recorded)
REM   run-e2e.bat --ui      — open Playwright interactive UI
REM   run-e2e.bat --report  — open last HTML report
REM ─────────────────────────────────────────────────────────────
setlocal

cd /d "%~dp0"

echo.
echo [1/4] Checking backend health...
curl -sf http://localhost:5000/api/v1/health >nul 2>&1
if errorlevel 1 (
  echo.
  echo  ERROR: Backend is not running at http://localhost:5000
  echo.
  echo  Start the backend first:
  echo    cd ..\backend ^&^& dotnet run --project RecruitIQ.API
  echo.
  exit /b 1
)
echo  OK - Backend is running.

echo.
echo [2/4] Installing dependencies...
if not exist node_modules (
  call npm install
)
call npx playwright install chromium --with-deps

echo.
echo [3/4] Running E2E tests...
echo.

if "%1"=="--ui" (
  call npx playwright test --ui
) else if "%1"=="--headless" (
  call npx playwright test
) else if "%1"=="--report" (
  call npx playwright show-report playwright-report
  goto :done
) else (
  REM headless:false is already set in playwright.config.ts
  call npx playwright test
)

echo.
echo [4/4] Opening HTML report...
call npx playwright show-report playwright-report

:done
endlocal
