# Coding Standards

All code in RecruitIQ AI must follow these standards. PRs that violate them will be blocked by CI or rejected in review.

---

## General Principles

- **SOLID** — Single Responsibility, Open/Closed, Liskov, Interface Segregation, Dependency Inversion
- **DRY** — Don't Repeat Yourself
- **KISS** — Keep It Simple
- **Loose coupling, high cohesion** — services depend on abstractions, not concretions
- **No magic numbers** — use named constants or config values
- **Fail fast** — validate at boundaries, throw early, not silently

---

## Backend — ASP.NET Core (.NET 8)

### Naming
```csharp
// Classes, interfaces, enums — PascalCase
public class CandidateService { }
public interface ICandidateRepository { }

// Methods — PascalCase
public async Task<CandidateDto> GetByIdAsync(Guid id) { }

// Variables, parameters — camelCase
var candidateList = await _repo.GetAllAsync();

// Constants — PascalCase
public const string DefaultRole = "Candidate";

// Private fields — _camelCase
private readonly ICandidateRepository _candidateRepository;
```

### Architecture — Clean Architecture Layers
```
backend/
├── API/              # Controllers, middleware, filters
├── Application/      # Use cases, DTOs, interfaces, validators
├── Domain/           # Entities, domain events, value objects
└── Infrastructure/   # EF Core, repositories, external services
```

### Dependency Injection
```csharp
// ALWAYS inject via constructor, never use service locator
public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _repo;
    private readonly ILogger<CandidateService> _logger;

    public CandidateService(ICandidateRepository repo, ILogger<CandidateService> logger)
    {
        _repo = repo;
        _logger = logger;
    }
}
```

### Async / Await
```csharp
// Always async all the way — never .Result or .Wait()
public async Task<CandidateDto> GetCandidateAsync(Guid id, CancellationToken ct)
{
    var candidate = await _repo.GetByIdAsync(id, ct);
    return _mapper.Map<CandidateDto>(candidate);
}
```

### Repository Pattern
```csharp
public interface ICandidateRepository
{
    Task<Candidate?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Candidate>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Candidate candidate, CancellationToken ct = default);
    Task UpdateAsync(Candidate candidate, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
```

### Response Patterns
```csharp
// Use Result<T> pattern — never throw business exceptions to controllers
public record Result<T>(bool IsSuccess, T? Value, string? Error)
{
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

### Rules
- No business logic in controllers — delegate to services/use cases
- No raw SQL — use EF Core LINQ or stored procedures via `FromSqlRaw`
- All endpoints return `ActionResult<T>` with proper HTTP status codes
- Use `CancellationToken` in all async repository and service methods
- Use `FluentValidation` for all request validation
- Use `AutoMapper` for entity ↔ DTO mapping

---

## Frontend — Angular 21

### File & Folder Structure
```
frontend/src/
├── app/
│   ├── core/           # Singleton services, guards, interceptors
│   ├── shared/         # Reusable components, pipes, directives
│   ├── features/       # Feature modules (lazy loaded)
│   │   ├── candidates/
│   │   ├── jobs/
│   │   └── dashboard/
│   └── layout/         # Shell, header, sidebar
├── environments/
└── assets/
```

### Naming Conventions
```
candidate-list.component.ts       // Components
candidate.service.ts              // Services
candidate.model.ts                // Models/interfaces
auth.guard.ts                     // Guards
token.interceptor.ts              // Interceptors
candidate-status.pipe.ts          // Pipes
```

### Component Rules
```typescript
// Use OnPush change detection for performance
@Component({
  selector: 'riq-candidate-card',
  templateUrl: './candidate-card.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CandidateCardComponent {
  // Use input signals (Angular 17+)
  candidate = input.required<Candidate>();
  selected = output<Candidate>();
}
```

### Services
```typescript
// Inject using inject() function (Angular 14+)
@Injectable({ providedIn: 'root' })
export class CandidateService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(API_BASE_URL);

  getCandidates(): Observable<Candidate[]> {
    return this.http.get<Candidate[]>(`${this.apiUrl}/candidates`);
  }
}
```

### Rules
- All feature modules must be **lazy loaded**
- No logic in component templates — use pipes or component methods
- Use **reactive forms** (not template-driven) for all forms
- All HTTP calls go through services — never in components
- Always unsubscribe using `takeUntilDestroyed()` or `async` pipe
- Use `@defer` blocks for heavy components

---

## AI Engine — Python FastAPI

### File Structure
```
ai-engine/
├── app/
│   ├── api/           # Route handlers
│   ├── core/          # Config, dependencies, security
│   ├── models/        # Pydantic models
│   ├── services/      # Business logic
│   └── repositories/  # Data access
├── tests/
├── main.py
└── requirements.txt
```

### Naming
```python
# Classes — PascalCase
class ResumeParserService:

# Functions, variables — snake_case
async def parse_resume(file_path: str) -> ResumeData:

# Constants — UPPER_SNAKE_CASE
MAX_RESUME_SIZE_MB = 5
```

### Type Hints — Always Required
```python
from pydantic import BaseModel

class CandidateScore(BaseModel):
    candidate_id: str
    score: float
    matched_skills: list[str]
    summary: str

async def score_candidate(resume_text: str, job_description: str) -> CandidateScore:
    ...
```

### Dependency Injection via FastAPI
```python
def get_resume_service(
    db: AsyncSession = Depends(get_db),
    llm: LLMClient = Depends(get_llm_client),
) -> ResumeService:
    return ResumeService(db=db, llm=llm)

@router.post("/parse")
async def parse_resume(
    file: UploadFile,
    service: ResumeService = Depends(get_resume_service),
) -> ResumeData:
    return await service.parse(file)
```

### Rules
- All endpoints must be `async`
- Use **Pydantic v2** for all request/response models
- Separate routers per domain (`/resumes`, `/candidates`, `/scores`)
- Never hard-code config — use `pydantic-settings` with `.env`
- All services must be testable in isolation (inject dependencies)

---

## Database — PostgreSQL + EF Core

### Naming
- Tables: `snake_case`, plural — `candidates`, `job_postings`
- Columns: `snake_case` — `first_name`, `created_at`
- PKs: `id` (UUID)
- FKs: `<table_singular>_id` — `candidate_id`
- Indexes: `ix_<table>_<column>`

### Migrations
```bash
# Always use named migrations
dotnet ef migrations add AddCandidateSkillsTable
dotnet ef database update
```

### Rules
- No lazy loading — always use `.Include()` explicitly
- Always index foreign keys
- Use `Guid` (UUID) for all primary keys — never `int`
- Soft delete using `IsDeleted` + `DeletedAt` columns — never hard delete
- All tables have `CreatedAt` and `UpdatedAt` timestamp columns
