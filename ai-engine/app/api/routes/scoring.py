import json
import logging
import re

from fastapi import APIRouter

from app.core.config import settings
from app.core.llm_client import llm_complete
from app.models.schemas import ResumeScoreRequest, ResumeScoreResponse

logger = logging.getLogger(__name__)
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


async def _llm_score(request: ResumeScoreRequest) -> ResumeScoreResponse | None:
    """
    Ask the LLM to score the resume and return structured JSON.
    Returns None if LLM is unavailable or response cannot be parsed.
    """
    skills_hint = (
        f"Required skills: {', '.join(request.required_skills)}\n" if request.required_skills else ""
    )
    prompt = f"""You are a recruitment AI assistant. Evaluate this resume against the job description and return ONLY valid JSON.

Job Description:
{request.job_description[:2000]}

{skills_hint}
Resume:
{request.resume_text[:3000]}

Return a JSON object with these exact keys:
{{
  "overall_score": <integer 0-100>,
  "skill_match_score": <integer 0-100>,
  "experience_score": <integer 0-100>,
  "education_score": <integer 0-100>,
  "matched_skills": ["skill1", "skill2"],
  "missing_skills": ["skill3", "skill4"],
  "match_reason": "<2-3 sentence explanation of fit>",
  "strengths": ["strength1", "strength2", "strength3"],
  "gaps": ["gap1", "gap2"],
  "summary": "<one sentence summary>"
}}

Respond with JSON only, no markdown, no explanation."""

    raw = await llm_complete(prompt)
    if raw is None:
        return None

    try:
        # Strip markdown code fences if present
        text = raw.strip()
        if text.startswith("```"):
            text = "\n".join(text.split("\n")[1:])
            text = text.rsplit("```", 1)[0]
        data = json.loads(text)
        return ResumeScoreResponse(
            candidate_id=request.candidate_id,
            overall_score=min(max(float(data.get("overall_score", 50)), 0), 100),
            skill_match_score=min(max(float(data.get("skill_match_score", 50)), 0), 100),
            experience_score=min(max(float(data.get("experience_score", 50)), 0), 100),
            education_score=min(max(float(data.get("education_score", 50)), 0), 100),
            matched_skills=data.get("matched_skills", []),
            missing_skills=data.get("missing_skills", []),
            summary=data.get("summary", ""),
            match_reason=data.get("match_reason", ""),
            strengths=data.get("strengths", []),
            gaps=data.get("gaps", []),
        )
    except (json.JSONDecodeError, KeyError, TypeError) as exc:
        logger.warning("Failed to parse LLM score response: %s", exc)
        return None


@router.post("/score", response_model=ResumeScoreResponse, tags=["Scoring"])
async def score_resume(request: ResumeScoreRequest) -> ResumeScoreResponse:
    """
    Score a candidate's resume against a job description.

    Uses LLM (OpenAI/Anthropic) when LLM_PROVIDER + LLM_API_KEY are configured.
    Falls back to regex/term-overlap scoring when LLM is unavailable.
    """
    # Try LLM first
    llm_result = await _llm_score(request)
    if llm_result is not None:
        return llm_result

    # Regex fallback
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

    match_reason, strengths, gaps = _build_reasoning(
        overall, blended_skill, exp_score, edu_score,
        matched_skills, missing_skills, jd_overlap,
    )

    return ResumeScoreResponse(
        candidate_id=request.candidate_id,
        overall_score=overall,
        skill_match_score=blended_skill,
        experience_score=exp_score,
        education_score=edu_score,
        matched_skills=matched_skills,
        missing_skills=missing_skills,
        summary=summary,
        match_reason=match_reason,
        strengths=strengths,
        gaps=gaps,
    )


def _build_reasoning(
    overall: float,
    skill_score: float,
    exp_score: float,
    edu_score: float,
    matched_skills: list[str],
    missing_skills: list[str],
    jd_overlap: float,
) -> tuple[str, list[str], list[str]]:
    """
    Generate a human-readable fit explanation, strengths list, and gaps list
    from the component scores — no external LLM required.
    """
    strengths: list[str] = []
    gaps: list[str] = []

    # ── Strengths ─────────────────────────────────────────────────────────────
    if skill_score >= 70:
        top = matched_skills[:4]
        strengths.append(f"Strong skill alignment — demonstrates {', '.join(top)}" if top else "Strong overall skill match")
    elif skill_score >= 40:
        top = matched_skills[:3]
        if top:
            strengths.append(f"Partial skill match — covers {', '.join(top)}")

    if exp_score >= 80:
        strengths.append("Substantial work experience relevant to the role")
    elif exp_score >= 55:
        strengths.append("Moderate level of relevant experience")

    if edu_score >= 85:
        strengths.append("Advanced academic background (Master's or PhD)")
    elif edu_score >= 70:
        strengths.append("Relevant educational qualification")

    if jd_overlap >= 60:
        strengths.append("Resume language closely aligns with the job description")

    # ── Gaps ──────────────────────────────────────────────────────────────────
    if missing_skills:
        listed = missing_skills[:4]
        gaps.append(f"Missing required skills: {', '.join(listed)}" + (" and others" if len(missing_skills) > 4 else ""))

    if exp_score < 40:
        gaps.append("Limited or unclear work experience")

    if edu_score < 50:
        gaps.append("Educational background may not meet role requirements")

    if skill_score < 30 and jd_overlap < 20:
        gaps.append("Resume content does not closely match the job description")

    # ── Narrative sentence ────────────────────────────────────────────────────
    if overall >= 75:
        fit_level = "strong"
        tone = "Highly recommended for further consideration."
    elif overall >= 50:
        fit_level = "moderate"
        tone = "Worth reviewing — shows relevant potential."
    elif overall >= 30:
        fit_level = "partial"
        tone = "May be considered for junior or adjacent roles."
    else:
        fit_level = "weak"
        tone = "Does not closely match the current requirement."

    skill_sentence = (
        f"The candidate matches {len(matched_skills)} of {len(matched_skills) + len(missing_skills)} required skills."
        if (matched_skills or missing_skills)
        else f"JD content overlap is {jd_overlap:.0f}%."
    )

    match_reason = (
        f"This candidate shows a {fit_level} fit for the role with an AI score of {overall:.0f}/100. "
        f"{skill_sentence} "
        f"{tone}"
    )

    return match_reason, strengths[:5], gaps[:4]
