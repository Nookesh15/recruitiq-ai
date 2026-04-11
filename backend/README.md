# Backend — ASP.NET Core Web API (.NET 8)

RecruitIQ AI REST API built with Clean Architecture.

## Prerequisites

- .NET 8 SDK
- PostgreSQL (or Docker)

## Setup

```bash
dotnet restore
dotnet build

# Apply migrations
dotnet ef database update --project Infrastructure --startup-project API

# Run
dotnet run --project API
# → http://localhost:5000
# → Swagger: http://localhost:5000/swagger
```

## Environment Config

Copy `appsettings.Development.json.example` to `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=recruitiq;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "Secret": "your-secret-key",
    "ExpiryMinutes": 60
  },
  "AiEngineUrl": "http://localhost:8000"
}
```

## Structure

```
backend/
├── API/              # Controllers, middleware, filters
├── Application/      # Use cases, DTOs, validators (MediatR, FluentValidation)
├── Domain/           # Entities, domain events, value objects
└── Infrastructure/   # EF Core, repositories, external services
```
