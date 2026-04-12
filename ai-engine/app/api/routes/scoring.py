import re

from fastapi import APIRouter

from app.core.config import settings
from app.models.schemas import ResumeScoreRequest, ResumeScoreResponse

router = APIRouter()


def _tokenize(text: str) -> set[str]:
    """Lowercase word-tokenize, strip punctuation."""
    return set(re.findall(r"\b[a-z][a-z0-9#+.]*\b", text.lower()))


def _skill_match(resume_tokens: set[str], required_skills: list[str]) -> tuple[float, list[str], list[str]]:
    """Return (score 0-100, matched, missing)."""
    if not required_skills:
        return 50.0, [], []

    matched = [s for s in required_skills if s.lower() in resume_tokens]
    missing = [s for s in required_skills if s.lower() not in resume_tokens]
    score = (len(matched) / len(required_skills)) * 100
    return round(score, 1), matched, missing


def _jd_similarity(resume_text: str, job_description: str) -> float:
    """Simple term-overlap similarity as a lightweight baseline (no ML deps at scaffold stage)."""
    resume_tokens = _tokenize(resume_text)
    jd_tokens = _tokenize(job_description)
    if not jd_tokens:
        return 0.0
    overlap = resume_tokens & jd_tokens
    score = (len(overlap) / len(jd_tokens)) * 100
    return min(round(score, 1), 100.0)


def _experience_score(resume_text: str) -> float:
    """Estimate years of experience from resume text."""
    years_pattern = re.findall(r"(\d+)\+?\s*(?:years?|yrs?)", resume_text.lower())
    if not years_pattern:
        return 50.0
    total_years = max(int(y) for y in years_pattern)
    # Cap at 15 years = 100 score
    return min(round((total_years / 15) * 100, 1), 100.0)


def _education_score(resume_text: str) -> float:
    """Score based on highest detected education level."""
    text = resume_text.lower()
    if any(kw in text for kw in ["phd", "ph.d", "doctorate"]):
        return 100.0
    if any(kw in text for kw in ["master", "mba", "m.s.", "m.e.", "m.tech"]):
        return 85.0
    if any(kw in text for kw in ["bachelor", "b.s.", "b.e.", "b.tech", "b.sc"]):
        return 70.0
    if any(kw in text for kw in ["associate", "diploma"]):
        return 50.0
    return 40.0


@router.post("/score", response_model=ResumeScoreResponse, tags=["Scoring"])
async def score_resume(request: ResumeScoreRequest) -> ResumeScoreResponse:
    """
    Score a candidate's resume against a job description.

    Returns an overall AI score (0-100) and breakdowns by skill match,
    experience, and education. This scaffold uses a term-overlap approach;
    replace with an embedding model (sentence-transformers) for production.
    """
    resume_tokens = _tokenize(request.resume_text)

    skill_score, matched_skills, missing_skills = _skill_match(
        resume_tokens, request.required_skills
    )
    jd_overlap = _jd_similarity(request.resume_text, request.job_description)
    # Blend skill match with JD overlap for the final skill score
    blended_skill = round((skill_score * 0.6 + jd_overlap * 0.4), 1)

    exp_score = _experience_score(request.resume_text)
    edu_score = _education_score(request.resume_text)

    overall = round(
        blended_skill * settings.skill_match_weight
        + exp_score * settings.experience_weight
        + edu_score * settings.education_weight,
        1,
    )

    summary = (
        f"Candidate matches {len(matched_skills)}/{len(request.required_skills)} required skills. "
        f"Overall AI score: {overall}/100."
    ) if request.required_skills else f"JD similarity score: {jd_overlap}/100. Overall: {overall}/100."

    return ResumeScoreResponse(
        candidate_id=request.candidate_id,
        overall_score=overall,
        skill_match_score=blended_skill,
        experience_score=exp_score,
        education_score=edu_score,
        matched_skills=matched_skills,
        missing_skills=missing_skills,
        summary=summary,
    )
