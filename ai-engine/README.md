# AI Engine — Python FastAPI

RecruitIQ AI microservice for resume parsing, candidate scoring, and bias detection.

## Prerequisites

- Python 3.11+
- pip

## Setup

```bash
python -m venv .venv
source .venv/bin/activate    # Windows: .venv\Scripts\activate

pip install -r requirements.txt

# Run
uvicorn main:app --reload --port 8000
# → http://localhost:8000
# → Docs: http://localhost:8000/docs
```

## Environment Config

Copy `.env.example` to `.env`:

```
OPENAI_API_KEY=sk-...
DATABASE_URL=postgresql+asyncpg://postgres:postgres@localhost/recruitiq
```

## Structure

```
ai-engine/
├── app/
│   ├── api/           # Route handlers
│   ├── core/          # Config, dependencies
│   ├── models/        # Pydantic models
│   ├── services/      # Business logic (parser, scorer, bias detector)
│   └── repositories/  # DB access
├── tests/
└── main.py
```

## Endpoints (Internal Only)

| Method | Path | Description |
|--------|------|-------------|
| POST | `/parse` | Extract structured data from resume |
| POST | `/score` | Score candidate against job description |
| POST | `/bias-check` | Detect biased language in JD |
