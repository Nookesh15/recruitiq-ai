# Deployment Guide

## Overview

RecruitIQ has three services:

| Service | Stack | Port |
|---------|-------|------|
| Backend API | .NET 9 ASP.NET Core | 5000 |
| AI Engine | Python FastAPI | 8000 |
| Frontend | Angular + nginx | 80 |

The database is **SQL Server** (Express edition for free tier, Developer edition for local dev).

---

## Option 1 — Railway (Recommended for demo)

Railway supports monorepo deployments with Docker. Free tier: 500 hours/month.

### Steps

1. **Create a Railway project**
   ```
   railway login
   railway init
   ```

2. **Add services** — Railway auto-detects from `railway.toml` in the repo root.

3. **Add a SQL Server plugin** in the Railway dashboard:
   - Click **+ New** → **Database** → **Microsoft SQL Server**
   - Copy the `DATABASE_URL` from the plugin's Variables tab.

4. **Set required environment variables** for the `backend` service:

   | Variable | Value |
   |----------|-------|
   | `ConnectionStrings__DefaultConnection` | From Railway SQL Server plugin |
   | `JWT_SECRET` | Random 32+ char string (`openssl rand -hex 32`) |
   | `AiEngineUrl` | Internal URL of the `ai-engine` service |
   | `AllowedOrigins` | Public URL of your frontend service |
   | `Email__Host` | SMTP host (optional — leave blank to disable emails) |

5. **Set for `ai-engine` service** (optional):

   | Variable | Value |
   |----------|-------|
   | `LLM_PROVIDER` | `openai` or `anthropic` (default: `none`) |
   | `LLM_API_KEY` | Your API key |

6. **Deploy**:
   ```
   railway up
   ```

7. **Verify** — Railway will run the healthcheck at `/api/v1/health` for each service.

---

## Option 2 — Docker Compose (Self-hosted / VPS)

### Prerequisites
- Docker Engine 24+ and Docker Compose v2
- A server with at least 2 GB RAM (SQL Server needs ~1 GB)

### Steps

1. **Clone the repo** on your server:
   ```bash
   git clone https://github.com/Nookesh15/recruitiq-ai.git
   cd recruitiq-ai
   ```

2. **Create the env file**:
   ```bash
   cp infrastructure/docker/.env.example infrastructure/docker/.env
   # Edit .env and fill in all required values
   ```

3. **Start the production stack**:
   ```bash
   docker compose -f infrastructure/docker/docker-compose.prod.yml up -d --build
   ```

4. **Check health**:
   ```bash
   curl http://localhost:5000/api/v1/health
   curl http://localhost:8000/api/v1/health
   curl http://localhost:80
   ```

5. **View logs**:
   ```bash
   docker compose -f infrastructure/docker/docker-compose.prod.yml logs -f backend
   ```

### Updating

```bash
git pull origin main
docker compose -f infrastructure/docker/docker-compose.prod.yml up -d --build --no-deps backend
```

---

## Option 3 — GitHub Actions → Render

The `.github/workflows/deploy-production.yml` workflow triggers on push to `main`:
- Frontend → **GitHub Pages**
- Backend → **Render** (via deploy hook)

### Required GitHub Secrets

| Secret | Description |
|--------|-------------|
| `RENDER_PROD_DEPLOY_HOOK` | Render deploy hook URL from your service settings |
| `PROD_API_URL` | Public URL of your backend API |

### Render Setup

1. Create a new **Web Service** on [render.com](https://render.com)
2. Connect your GitHub repo
3. Set **Root Directory** to `backend`
4. Set **Build Command**: `dotnet publish -c Release -o out`
5. Set **Start Command**: `dotnet out/RecruitIQ.API.dll`
6. Add environment variables matching `.env.example`
7. Copy the **Deploy Hook URL** into the `RENDER_PROD_DEPLOY_HOOK` GitHub secret

---

## Default Admin Credentials

The app seeds a default admin account on first startup:

| Field | Value |
|-------|-------|
| Email | `admin@recruitiq.ai` |
| Password | `Admin@123` |

**Change this immediately after first login in production.**

---

## Health Endpoints

| Service | Endpoint | Expected |
|---------|----------|---------|
| Backend | `GET /api/v1/health` | `{"status":"healthy"}` |
| AI Engine | `GET /api/v1/health` | `{"status":"ok"}` |
| Frontend | `GET /` | HTTP 200 |
