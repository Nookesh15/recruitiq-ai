# Infrastructure

Docker and deployment configuration for RecruitIQ AI.

## Local Development with Docker Compose

```bash
# Start all services (PostgreSQL + Backend + AI Engine + Frontend)
docker compose up -d

# Services:
# Frontend   → http://localhost:4200
# Backend    → http://localhost:5000
# AI Engine  → http://localhost:8000
# PostgreSQL → localhost:5432
```

## Environment Files

Copy `docker/.env.example` to `docker/.env` before running.

## Production Deployment

| Service | Platform | Trigger |
|---------|----------|---------|
| Frontend | GitHub Pages | Push to `main` |
| Backend | Render | Push to `main` (webhook) |
| AI Engine | Fly.io | Push to `main` |
| Database | Supabase | Managed |
