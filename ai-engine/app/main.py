from contextlib import asynccontextmanager
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.core.config import settings
from app.api.routes import health, scoring


@asynccontextmanager
async def lifespan(app: FastAPI):  # type: ignore[type-arg]
    # Startup: warm up models / connections here
    print(f"🚀 {settings.app_name} v{settings.app_version} starting up")
    yield
    # Shutdown
    print("👋 Shutting down AI engine")


app = FastAPI(
    title=settings.app_name,
    version=settings.app_version,
    description="AI-powered resume scoring and candidate ranking engine for RecruitIQ",
    docs_url="/docs",
    redoc_url="/redoc",
    lifespan=lifespan,
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:4200", "http://localhost:5000"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(health.router, prefix="/api/v1")
app.include_router(scoring.router, prefix="/api/v1")
