# Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────┐
│                        Clients                          │
│              Browser (Angular 21 SPA)                   │
└────────────────────────┬────────────────────────────────┘
                         │ HTTPS
┌────────────────────────▼────────────────────────────────┐
│              ASP.NET Core Web API (.NET 8)               │
│         REST endpoints, Auth (JWT), Business Logic       │
│              Clean Architecture (4 layers)               │
└──────┬─────────────────┬──────────────────┬─────────────┘
       │ EF Core          │ HttpClient        │ SignalR
┌──────▼──────┐  ┌────────▼────────┐  ┌──────▼──────┐
│ PostgreSQL  │  │ Python FastAPI  │  │  Real-time  │
│  (Supabase) │  │   AI Engine     │  │  Notifs     │
└─────────────┘  └─────────────────┘  └─────────────┘
```

---

## Layer Responsibilities

### Frontend — Angular 21
- SPA served from GitHub Pages / Vercel
- Communicates with Backend via REST (HttpClient)
- Real-time updates via SignalR
- Feature-module architecture with lazy loading
- OnPush change detection throughout

### Backend — ASP.NET Core Web API
Follows **Clean Architecture**:

```
API Layer          → Controllers, Middleware, Filters
Application Layer  → Use Cases (CQRS with MediatR), DTOs, Validators
Domain Layer       → Entities, Domain Events, Value Objects, Interfaces
Infrastructure     → EF Core, Repositories, External HTTP clients
```

- JWT authentication with refresh tokens
- Role-based authorization (Admin / Recruiter / Candidate)
- All AI calls proxied through this API (Frontend never calls AI engine directly)

### AI Engine — Python FastAPI
- Separate microservice, not exposed to public internet
- Called only by the Backend API
- Responsibilities:
  - PDF/DOCX resume text extraction
  - LLM-based resume parsing (skills, experience, education)
  - Candidate-to-JD scoring
  - Bias detection in job descriptions

### Database — PostgreSQL (Supabase)
- Single database, schema-separated by domain
- EF Core Code-First with migrations
- UUID primary keys throughout
- Soft delete pattern (`IsDeleted`, `DeletedAt`)

---

## Key Design Decisions

| Decision | Choice | Reason |
|----------|--------|--------|
| API style | REST | Familiar, widely supported, simple |
| Auth | JWT + Refresh Token | Stateless, scalable |
| ORM | EF Core + Npgsql | Native .NET, strong migration support |
| AI isolation | Separate microservice | Keep .NET API clean, AI can scale independently |
| Real-time | SignalR | Native to .NET, good Angular support |
| State management | Angular Signals | No NgRx complexity for this scale |

---

## Data Flow: Resume Screening

```
1. Recruiter uploads PDF via Angular UI
2. Angular → POST /api/resumes (Backend)
3. Backend stores file, creates Candidate record
4. Backend → POST /internal/parse (AI Engine)
5. AI Engine extracts text, parses with LLM
6. AI Engine → POST /internal/score (AI Engine)
7. Score returned to Backend
8. Backend saves score to PostgreSQL
9. Backend notifies Frontend via SignalR
10. Angular updates candidate score in real-time
```

---

## Environments

| Environment | Branch | Frontend | Backend | Database |
|-------------|--------|----------|---------|----------|
| Local Dev | any | localhost:4200 | localhost:5000 | Docker PostgreSQL |
| Staging | develop | Vercel Preview | Render (staging) | Supabase (staging) |
| Production | main | GitHub Pages | Render (prod) | Supabase (prod) |
