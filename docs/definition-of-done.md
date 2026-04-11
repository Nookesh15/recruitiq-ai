# Definition of Done

A ticket is **Done** only when ALL of the following are true:

---

## Code

- [ ] Feature works as described in the acceptance criteria
- [ ] Code follows [Coding Standards](coding-standards.md)
- [ ] No commented-out code or debug statements (`console.log`, `Debug.WriteLine`, `print`)
- [ ] No hardcoded secrets, URLs, or environment-specific values
- [ ] Breaking changes documented in PR description

## Tests

- [ ] Unit tests written for new business logic
- [ ] All existing tests pass (CI is green)
- [ ] Edge cases and error paths covered
- [ ] Test coverage does not decrease

## Review

- [ ] PR linked to a GitHub Issue
- [ ] PR title follows Conventional Commits format
- [ ] PR description explains **what** changed and **why**
- [ ] At least **1 approval** received
- [ ] All review comments resolved or acknowledged

## Integration

- [ ] Feature works end-to-end in the staging environment
- [ ] No regressions in adjacent features
- [ ] Database migrations run cleanly (if applicable)
- [ ] API contract matches frontend expectations

## Documentation

- [ ] API changes reflected in [api-reference.md](api-reference.md)
- [ ] `CHANGELOG.md` updated (for user-facing changes)
- [ ] README updated if setup steps changed

## CI/CD

- [ ] All GitHub Actions checks pass
- [ ] Branch is up to date with target branch
- [ ] No merge conflicts

---

> If any item above is not checked, the ticket stays **In Review** — not Done.
