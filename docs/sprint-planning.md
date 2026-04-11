# Sprint Planning

## Sprint Structure

- **Length:** 2 weeks
- **Ceremony cadence:**

| Ceremony | When | Duration |
|----------|------|----------|
| Sprint Planning | Monday, Week 1 | 2 hours |
| Daily Standup | Every day | 15 min |
| Sprint Review | Friday, Week 2 | 1 hour |
| Retrospective | Friday, Week 2 | 45 min |
| Backlog Grooming | Wednesday, Week 1 | 1 hour |

---

## Story Points (Fibonacci)

| Points | Effort |
|--------|--------|
| 1 | Trivial — config change, typo fix |
| 2 | Simple — well-understood, < 2 hours |
| 3 | Small — clear path, half a day |
| 5 | Medium — some unknowns, 1–2 days |
| 8 | Large — significant complexity, 3–4 days |
| 13 | Very Large — break it down further |

> Tickets larger than 8 points **must be broken down** before sprint start.

---

## GitHub Labels

| Label | Color | Purpose |
|-------|-------|---------|
| `type: feature` | `#0075ca` | New functionality |
| `type: bug` | `#d73a4a` | Something is broken |
| `type: chore` | `#e4e669` | Tooling, CI, config |
| `type: docs` | `#0052cc` | Documentation only |
| `type: refactor` | `#cfd3d7` | Code restructure |
| `priority: critical` | `#b60205` | P0 — must ship this sprint |
| `priority: high` | `#e99695` | P1 — important |
| `priority: medium` | `#f9d0c4` | P2 — normal |
| `priority: low` | `#fef2c0` | P3 — nice to have |
| `layer: frontend` | `#bfd4f2` | Angular work |
| `layer: backend` | `#d4c5f9` | .NET API work |
| `layer: ai-engine` | `#c2e0c6` | Python AI work |
| `layer: infra` | `#f5c6a0` | Docker, CI/CD, infra |
| `layer: db` | `#e6dcf9` | Database, migrations |
| `status: in-progress` | `#ededed` | Actively being worked |
| `status: blocked` | `#e11d48` | Waiting on dependency |
| `status: review` | `#0e8a16` | PR open, needs review |
| `size: S` | `#c2e0c6` | 1–3 points |
| `size: M` | `#fef2c0` | 5 points |
| `size: L` | `#f9d0c4` | 8 points |

---

## Sprint 1 — Foundation (Weeks 1–2)

**Goal:** Repository setup, project scaffolding, CI/CD pipeline, dev environment.

| # | Ticket | Layer | Points |
|---|--------|-------|--------|
| RIQAI-1 | Set up repo, branching rules, templates | infra | 2 |
| RIQAI-2 | Scaffold Angular 21 frontend | frontend | 3 |
| RIQAI-3 | Scaffold ASP.NET Core Web API | backend | 3 |
| RIQAI-4 | Scaffold Python FastAPI AI engine | ai-engine | 2 |
| RIQAI-5 | Set up PostgreSQL + EF Core + migrations | db | 3 |
| RIQAI-6 | Docker Compose for local dev | infra | 3 |
| RIQAI-7 | GitHub Actions CI pipeline | infra | 3 |
| RIQAI-8 | Set up GitHub Project board + labels | infra | 1 |

**Total:** 20 points

---

## Sprint 2 — Auth & Core API (Weeks 3–4)

**Goal:** Authentication, user roles, core entity APIs.

| # | Ticket | Layer | Points |
|---|--------|-------|--------|
| RIQAI-9 | JWT auth — register/login endpoints | backend | 5 |
| RIQAI-10 | Role-based access (Admin, Recruiter, Candidate) | backend | 5 |
| RIQAI-11 | Angular auth module + login page | frontend | 5 |
| RIQAI-12 | Auth guard + token interceptor | frontend | 3 |
| RIQAI-13 | Candidate CRUD API | backend | 5 |
| RIQAI-14 | Job Posting CRUD API | backend | 5 |

**Total:** 28 points

---

## Sprint 3 — AI Resume Screening (Weeks 5–6)

**Goal:** Upload resumes, AI parsing, candidate scoring against JD.

| # | Ticket | Layer | Points |
|---|--------|-------|--------|
| RIQAI-15 | Resume upload endpoint (PDF/DOCX) | backend | 3 |
| RIQAI-16 | PDF/DOCX text extraction | ai-engine | 5 |
| RIQAI-17 | LLM-based resume parser (skills, exp, education) | ai-engine | 8 |
| RIQAI-18 | Candidate-to-JD scoring algorithm | ai-engine | 8 |
| RIQAI-19 | Score results stored to PostgreSQL | backend | 3 |
| RIQAI-20 | Resume upload UI + score display | frontend | 5 |

**Total:** 32 points

---

## Sprint 4 — Dashboard & Analytics (Weeks 7–8)

**Goal:** Recruiter dashboard, hiring funnel metrics, candidate pipeline view.

| # | Ticket | Layer | Points |
|---|--------|-------|--------|
| RIQAI-21 | Dashboard layout + navigation | frontend | 3 |
| RIQAI-22 | Candidate pipeline Kanban view | frontend | 8 |
| RIQAI-23 | Hiring funnel metrics API | backend | 5 |
| RIQAI-24 | Analytics charts (time-to-hire, source quality) | frontend | 5 |
| RIQAI-25 | Bias detection flag on JD text | ai-engine | 5 |

**Total:** 26 points
