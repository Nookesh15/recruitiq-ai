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

## Sprint 2 — Core Features (Weeks 3–4) ✅

**Goal:** JWT auth, candidate/job management, resume AI scoring, public apply portal, dashboard.

> Delivered ahead of schedule — Sprint 2 scope expanded to cover original Sprint 3 & 4 foundations.

| # | Ticket | Layer | Points | Status |
|---|--------|-------|--------|--------|
| RIQAI-9 | JWT authentication — login/register | backend | 5 | ✅ Done |
| RIQAI-10 | Candidate management CRUD + detail page | backend+frontend | 8 | ✅ Done |
| RIQAI-11 | Job Postings CRUD + status management | backend+frontend | 8 | ✅ Done |
| RIQAI-12 | Resume upload + basic AI scoring | ai-engine+backend | 5 | ✅ Done |
| RIQAI-13 | Job Applications tracking | backend+frontend | 5 | ✅ Done |
| RIQAI-14 | Dashboard with real-time stats | backend+frontend | 5 | ✅ Done |
| RIQAI-15 | Public job apply portal | backend+frontend | 5 | ✅ Done |
| RIQAI-16 | Job applicants view ranked by AI score | backend+frontend | 3 | ✅ Done |
| RIQAI-17 | Bug: candidate navigation double-click | frontend | 3 | ✅ Closed |

**Total:** 47 points

---

## Sprint 3 — AI Resume Screening (Weeks 5–6)

**Goal:** Structured LLM parsing, match reasoning, bias detection, fix navigation bug.

| # | Ticket | Layer | Points | Issue |
|---|--------|-------|--------|-------|
| RIQAI-18 | Structured resume parsing (skills, experience, education) | ai-engine | 8 | #27 |
| RIQAI-19 | AI match score with reasoning — explain candidate fit | ai-engine | 8 | #28 |
| RIQAI-20 | Bias detection on job description text | ai-engine | 5 | #29 |
| RIQAI-21 | Candidate detail — show parsed resume sections | frontend | 5 | #30 |
| RIQAI-22 | Fix single-click candidate navigation bug | frontend | 3 | #31 |

**Total:** 29 points

---

## Sprint 4 — Dashboard & Analytics (Weeks 7–8)

**Goal:** Kanban pipeline, hiring funnel metrics, analytics charts, roles, notifications.

| # | Ticket | Layer | Points | Issue |
|---|--------|-------|--------|-------|
| RIQAI-23 | Candidate pipeline Kanban view (drag-and-drop) | frontend | 8 | #32 |
| RIQAI-24 | Hiring funnel metrics API | backend | 5 | #33 |
| RIQAI-25 | Analytics charts (funnel, time-to-hire, pipeline) | frontend | 5 | #34 |
| RIQAI-26 | Role-based access control (Admin vs Recruiter) | backend | 5 | #35 |
| RIQAI-27 | Email notifications (application events) | backend | 5 | #36 |

**Total:** 28 points

---

## Sprint 5 — Quality & Deployment (Weeks 9–10)

**Goal:** Fix the AI pipeline (PDF extraction), add real LLM intelligence, backend tests, form validation, and production deployment.

| # | Ticket | Layer | Points | Issue |
|---|--------|-------|--------|-------|
| RIQAI-28 | PDF text extraction on resume upload | backend | 5 | #37 |
| RIQAI-29 | LLM integration — OpenAI/Claude fallback in AI engine | ai-engine | 8 | #38 |
| RIQAI-30 | Backend unit tests — xUnit for critical handlers | backend | 5 | #39 |
| RIQAI-31 | Frontend form validation — reactive forms | frontend | 3 | #40 |
| RIQAI-32 | Production deployment — Railway config + deploy guide | infra | 5 | #41 |

**Total:** 26 points
