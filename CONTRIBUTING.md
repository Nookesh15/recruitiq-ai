# Contributing to RecruitIQ AI

Thank you for contributing! Please follow these standards to keep the codebase consistent, scalable, and production-ready.

---

## Table of Contents

- [Branching](#branching)
- [Commit Messages](#commit-messages)
- [Pull Requests](#pull-requests)
- [Code Review](#code-review)
- [Issue Linking](#issue-linking)

---

## Branching

Always branch from the correct base. See [docs/branching-strategy.md](docs/branching-strategy.md) for the full flow.

### Branch Naming Convention

```
<type>/RIQAI-<issue-number>-<short-description>
```

| Type | When to use |
|------|------------|
| `feature/` | New feature or enhancement |
| `bugfix/` | Bug fix on develop |
| `hotfix/` | Critical fix directly on main |
| `release/` | Release preparation branch |
| `chore/` | Tooling, config, CI updates |
| `docs/` | Documentation only changes |

**Examples:**
```
feature/RIQAI-42-resume-parser
bugfix/RIQAI-78-fix-jwt-expiry
hotfix/RIQAI-99-null-ref-candidate
release/v1.2.0
docs/RIQAI-55-update-api-reference
```

---

## Commit Messages

Follow the **Conventional Commits** specification: [conventionalcommits.org](https://www.conventionalcommits.org)

### Format

```
<type>(<scope>): <short description>

[optional body]

[optional footer: RIQAI-123]
```

### Types

| Type | Purpose |
|------|---------|
| `feat` | New feature |
| `fix` | Bug fix |
| `docs` | Documentation only |
| `style` | Formatting, no logic change |
| `refactor` | Code restructure, no feature/fix |
| `test` | Adding or updating tests |
| `chore` | Build, CI, dependency updates |
| `perf` | Performance improvement |

### Scopes

`frontend` | `backend` | `ai-engine` | `infra` | `auth` | `db` | `ci`

### Examples

```
feat(backend): add candidate ranking endpoint
fix(frontend): resolve resume upload timeout
docs(api-reference): add interview scheduler endpoints
chore(ci): add PR title lint workflow
refactor(ai-engine): decouple resume parser from scorer
```

### Rules

- Use **imperative, present tense**: "add" not "added" or "adds"
- No period at the end of the subject line
- Subject line max **72 characters**
- Reference the issue in footer: `Refs: RIQAI-123` or `Closes: RIQAI-123`

---

## Pull Requests

### Before Opening a PR

- [ ] Branch is up to date with `develop` (or `main` for hotfixes)
- [ ] All tests pass locally
- [ ] No console errors or warnings
- [ ] Self-reviewed your own diff
- [ ] Linked to a GitHub Issue

### PR Title Format

Must follow Conventional Commits format (enforced by CI):

```
feat(backend): add resume screening AI endpoint [RIQAI-42]
```

### PR Size

Keep PRs small and focused. A good PR:
- Changes **one concern** at a time
- Is reviewable in **under 30 minutes**
- Has a clear description of **what** and **why**

### Rules

- Minimum **1 approval** required before merge
- CI must be **green** before merge
- No direct commits to `main` or `develop`
- Use **Squash and Merge** for feature branches into develop
- Use **Merge Commit** for release branches into main

---

## Code Review

### As a Reviewer

- Review within **24 hours** of assignment
- Be constructive — suggest, don't dictate
- Distinguish between blocking (`MUST`) and optional (`NIT`) feedback
- Approve only when you'd be comfortable shipping the code

### As an Author

- Respond to all comments before re-requesting review
- Don't force-push after review has started
- Mark conversations resolved only after addressing them

---

## Issue Linking

Always link your PR to a GitHub Issue:

```
Closes #42
Refs #78
```

Use `Closes` when the PR fully resolves the issue. Use `Refs` when it partially addresses it.
