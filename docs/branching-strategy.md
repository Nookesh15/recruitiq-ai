# Branching Strategy

RecruitIQ AI follows a **Git Flow** branching model adapted for CI/CD automation.

---

## Branch Overview

```
main
 └── release/v*
      └── develop
           ├── feature/RIQAI-*
           ├── bugfix/RIQAI-*
           └── chore/RIQAI-*
hotfix/RIQAI-*  ──► main + develop
```

---

## Branches

### `main`
- **Represents:** Production
- **Protected:** Yes — no direct commits, requires PR + 1 approval + passing CI
- **Merges from:** `release/*` or `hotfix/*` only
- **Auto-deploys to:** Production (Render / GitHub Pages)
- **Tagged:** Every merge creates a version tag (e.g., `v1.2.0`)

### `develop`
- **Represents:** Staging / integration
- **Protected:** Yes — no direct commits, requires PR + 1 approval + passing CI
- **Merges from:** `feature/*`, `bugfix/*`, `chore/*`, `docs/*`
- **Auto-deploys to:** Staging environment

### `feature/RIQAI-<id>-<description>`
- **Base:** `develop`
- **Merges into:** `develop`
- **Lifetime:** Deleted after merge
- **Example:** `feature/RIQAI-42-resume-parser`

### `bugfix/RIQAI-<id>-<description>`
- **Base:** `develop`
- **Merges into:** `develop`
- **Example:** `bugfix/RIQAI-78-fix-jwt-expiry`

### `hotfix/RIQAI-<id>-<description>`
- **Base:** `main`
- **Merges into:** `main` AND `develop`
- **Use when:** Critical production bug needs immediate fix
- **Example:** `hotfix/RIQAI-99-null-ref-crash`

### `release/v<major>.<minor>.<patch>`
- **Base:** `develop`
- **Merges into:** `main` AND `develop`
- **Use when:** Preparing a production release (final testing, version bump, changelog)
- **Example:** `release/v1.2.0`

### `chore/*` / `docs/*`
- **Base:** `develop`
- **Merges into:** `develop`
- **Use for:** CI changes, tooling, documentation updates

---

## Workflow: Feature Development

```bash
# 1. Sync develop
git checkout develop
git pull origin develop

# 2. Create feature branch
git checkout -b feature/RIQAI-42-resume-parser

# 3. Develop, commit using Conventional Commits
git commit -m "feat(ai-engine): add PDF resume text extractor"

# 4. Push and open PR to develop
git push origin feature/RIQAI-42-resume-parser
# → Open PR on GitHub targeting develop
```

## Workflow: Hotfix

```bash
# 1. Branch from main
git checkout main && git pull origin main
git checkout -b hotfix/RIQAI-99-null-ref-crash

# 2. Fix, commit, push
git commit -m "fix(backend): handle null candidate object in ranking service"
git push origin hotfix/RIQAI-99-null-ref-crash

# 3. Open TWO PRs: one to main, one to develop
```

## Workflow: Release

```bash
# 1. Branch from develop
git checkout develop && git pull
git checkout -b release/v1.2.0

# 2. Bump version, update CHANGELOG.md
# 3. Open PR to main → merge → tag v1.2.0
# 4. Open PR to develop → merge
```

---

## Merge Strategy

| Source | Target | Strategy |
|--------|--------|----------|
| `feature/*` | `develop` | Squash and Merge |
| `bugfix/*` | `develop` | Squash and Merge |
| `release/*` | `main` | Merge Commit |
| `release/*` | `develop` | Merge Commit |
| `hotfix/*` | `main` | Merge Commit |
| `hotfix/*` | `develop` | Merge Commit |

---

## Branch Protection Rules

Both `main` and `develop` enforced on GitHub:

- Require pull request before merging
- Require at least 1 approving review
- Dismiss stale reviews when new commits are pushed
- Require status checks to pass (CI must be green)
- Require branches to be up to date before merging
- No force pushes
- No branch deletions
