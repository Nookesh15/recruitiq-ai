# RecruitIQ AI

> AI-powered intelligent recruitment platform — resume screening, candidate matching, interview scheduling, and hiring analytics.

[![CI](https://github.com/Nookesh15/recruitiq-ai/actions/workflows/ci.yml/badge.svg)](https://github.com/Nookesh15/recruitiq-ai/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

RecruitIQ AI transforms the end-to-end recruitment lifecycle using machine learning and NLP — helping HR teams make faster, bias-reduced, data-driven hiring decisions.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Angular 21, TypeScript, TailwindCSS |
| Backend | ASP.NET Core Web API (.NET 8) |
| Database | PostgreSQL + Entity Framework Core (Npgsql) |
| AI Engine | Python 3.11, FastAPI, LangChain |
| Auth | ASP.NET Identity + JWT |
| Hosting | Render (API), Supabase (DB), GitHub Pages (Frontend) |
| CI/CD | GitHub Actions |

## Project Structure

```
recruitiq-ai/
├── frontend/        # Angular 21 web app
├── backend/         # ASP.NET Core Web API
├── ai-engine/       # Python FastAPI AI microservice
├── infrastructure/  # Docker, deployment configs
└── docs/            # Architecture, standards, guides
```

## Quick Start

See [docs/setup.md](docs/setup.md) for full local development setup.

## Documentation

- [Architecture](docs/architecture.md)
- [Branching Strategy](docs/branching-strategy.md)
- [Coding Standards](docs/coding-standards.md)
- [Sprint Planning](docs/sprint-planning.md)
- [Definition of Done](docs/definition-of-done.md)
- [API Reference](docs/api-reference.md)
- [Contributing](CONTRIBUTING.md)
- [Changelog](CHANGELOG.md)

## License

MIT License — see [LICENSE](LICENSE) for details.
