import pytest
from httpx import ASGITransport, AsyncClient

from app.main import app

SAMPLE_RESUME = """
John Doe
Senior Software Engineer — 7 years experience

Skills: Python, FastAPI, SQL Server, Docker, Kubernetes, React, TypeScript

Education: Bachelor of Science in Computer Science, State University 2017

Experience:
- Lead Engineer at TechCorp (2020-2024): Designed microservices using Python and FastAPI
- Software Engineer at StartupXYZ (2017-2020): Built REST APIs with Python and Flask
"""

SAMPLE_JD = """
We are looking for a Senior Python Engineer with experience in FastAPI, SQL databases,
Docker, and cloud platforms. Strong communication skills required.
"""


@pytest.mark.asyncio
async def test_score_resume_returns_valid_score() -> None:
    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as client:
        response = await client.post(
            "/api/v1/score",
            json={
                "candidate_id": "test-uuid-1234",
                "resume_text": SAMPLE_RESUME,
                "job_description": SAMPLE_JD,
                "required_skills": ["Python", "FastAPI", "Docker", "SQL Server"],
            },
        )
    assert response.status_code == 200
    body = response.json()
    assert body["candidate_id"] == "test-uuid-1234"
    assert 0 <= body["overall_score"] <= 100
    assert isinstance(body["matched_skills"], list)
    assert isinstance(body["missing_skills"], list)
    assert "summary" in body


@pytest.mark.asyncio
async def test_score_resume_without_skills() -> None:
    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as client:
        response = await client.post(
            "/api/v1/score",
            json={
                "candidate_id": "test-uuid-5678",
                "resume_text": SAMPLE_RESUME,
                "job_description": SAMPLE_JD,
                "required_skills": [],
            },
        )
    assert response.status_code == 200
    body = response.json()
    assert 0 <= body["overall_score"] <= 100


@pytest.mark.asyncio
async def test_score_resume_validation_error() -> None:
    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as client:
        response = await client.post(
            "/api/v1/score",
            json={
                "candidate_id": "test",
                "resume_text": "x",  # too short
                "job_description": "y",  # too short
            },
        )
    assert response.status_code == 422
