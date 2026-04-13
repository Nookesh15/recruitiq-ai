# Infrastructure

Docker and deployment configuration for RecruitIQ AI.

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (v24+)
- At least 4 GB RAM allocated to Docker (SQL Server requires ~2 GB)

---

## Local Development with Docker Compose

### 1. Create your `.env` file

```bash
cd infrastructure/docker
cp .env.example .env
# Edit .env — set MSSQL_SA_PASSWORD and OPENAI_API_KEY
```

### 2. Start all services

```bash
docker compose up --build
```

This starts four services:

| Service    | URL                          | Description                        |
|------------|------------------------------|------------------------------------|
| Frontend   | http://localhost:4200        | Angular app served via nginx       |
| Backend    | http://localhost:5000        | .NET 9 REST API (Swagger at `/swagger`) |
| AI Engine  | http://localhost:8000        | Python FastAPI resume scoring      |
| SQL Server | `localhost,1433`             | SQL Server 2022 Developer Edition  |

EF Core migrations run automatically on backend startup — no manual `dotnet ef` step needed.

### 3. Default credentials

| Field    | Value           |
|----------|-----------------|
| Email    | `admin@recruitiq.ai` |
| Password | `Admin@123`     |

### Useful commands

```bash
# Start in background (detached)
docker compose up -d --build

# View logs for a specific service
docker compose logs -f backend

# Stop everything (keep data)
docker compose down

# Stop and wipe the database volume
docker compose down -v

# Rebuild only one service after a code change
docker compose up --build backend
```

---

## Architecture

```
┌──────────────┐     ┌───────────────────┐     ┌───────────────────┐
│   Angular    │────▶│  .NET 9 API       │────▶│  SQL Server 2022  │
│  (nginx:80)  │     │  (Kestrel:5000)   │     │  (port 1433)      │
└──────────────┘     └─────────┬─────────┘     └───────────────────┘
                               │
                               ▼
                     ┌───────────────────┐
                     │  Python FastAPI   │
                     │  AI Engine:8000   │
                     └───────────────────┘
```

- **nginx** proxies `/api/` calls from the Angular app to the backend container (no CORS needed in Docker mode)
- **EF Core** auto-migrates the database on startup
- **SQL Server** data is persisted in a named Docker volume (`mssql_data`)

---

## Production Deployment

| Service    | Platform       | Trigger              |
|------------|----------------|----------------------|
| Frontend   | GitHub Pages   | Push to `main`       |
| Backend    | Render / Azure | Push to `main`       |
| AI Engine  | Fly.io         | Push to `main`       |
| Database   | Azure SQL      | Managed              |
