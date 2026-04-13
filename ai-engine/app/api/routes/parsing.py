"""
RIQAI-18: Structured resume parsing
Extracts skills, work experience, education, and a summary from raw resume text.
Uses regex + curated pattern matching — no external API required.
"""
from __future__ import annotations

import re
from datetime import datetime, timezone

from fastapi import APIRouter

from app.models.schemas import ParseResumeRequest, ParseResumeResponse, ParsedExperience, ParsedEducation

router = APIRouter()

# ── Curated tech / professional skills list ───────────────────────────────────
_TECH_SKILLS: list[str] = [
    # Languages
    "python", "java", "javascript", "typescript", "c#", "c++", "c", "go", "rust",
    "kotlin", "swift", "ruby", "php", "scala", "r", "matlab", "dart", "elixir",
    # Frontend
    "angular", "react", "vue", "svelte", "html", "css", "sass", "scss", "tailwind",
    "bootstrap", "jquery", "next.js", "nuxt", "gatsby",
    # Backend
    "node.js", "express", "fastapi", "django", "flask", "spring", "asp.net", ".net",
    "laravel", "rails", "nestjs",
    # Databases
    "sql", "mysql", "postgresql", "sqlite", "mongodb", "redis", "elasticsearch",
    "cassandra", "dynamodb", "oracle", "sql server", "mssql", "cosmos db",
    # Cloud / DevOps
    "aws", "azure", "gcp", "docker", "kubernetes", "terraform", "ansible",
    "jenkins", "github actions", "gitlab ci", "ci/cd", "linux", "bash",
    # AI / ML
    "machine learning", "deep learning", "nlp", "computer vision", "tensorflow",
    "pytorch", "scikit-learn", "pandas", "numpy", "keras", "langchain", "openai",
    "hugging face", "transformers",
    # Mobile
    "android", "ios", "react native", "flutter", "xamarin",
    # Tools / practices
    "git", "jira", "confluence", "agile", "scrum", "rest", "graphql", "grpc",
    "microservices", "kafka", "rabbitmq", "celery", "websockets",
    # Soft skills
    "leadership", "communication", "teamwork", "problem solving", "project management",
]

# Normalised for matching
_SKILL_SET = {s.lower() for s in _TECH_SKILLS}
_SKILL_DISPLAY = {s.lower(): s for s in _TECH_SKILLS}


def _extract_skills(text: str) -> list[str]:
    """Return matched skills preserving display-case, deduplicated, sorted."""
    text_lower = text.lower()
    found: dict[str, str] = {}
    for skill_lower, skill_display in _SKILL_DISPLAY.items():
        # Match as whole word / phrase (handle "c#", "c++", ".net" specially)
        escaped = re.escape(skill_lower)
        if re.search(rf"(?<![a-z0-9]){escaped}(?![a-z0-9])", text_lower):
            found[skill_lower] = skill_display
    return sorted(found.values(), key=str.lower)


# ── Experience extraction ─────────────────────────────────────────────────────

# Patterns for date ranges: "Jan 2020 – Mar 2023", "2019 - Present", "2018–2020"
_DATE_RANGE_RE = re.compile(
    r"(?:(?:jan(?:uary)?|feb(?:ruary)?|mar(?:ch)?|apr(?:il)?|may|jun(?:e)?|"
    r"jul(?:y)?|aug(?:ust)?|sep(?:t(?:ember)?)?|oct(?:ober)?|nov(?:ember)?|"
    r"dec(?:ember)?)\s+)?\d{4}"
    r"\s*[-–—to]+\s*"
    r"(?:(?:jan(?:uary)?|feb(?:ruary)?|mar(?:ch)?|apr(?:il)?|may|jun(?:e)?|"
    r"jul(?:y)?|aug(?:ust)?|sep(?:t(?:ember)?)?|oct(?:ober)?|nov(?:ember)?|"
    r"dec(?:ember)?)\s+)?\d{4}|present|current|now",
    re.IGNORECASE,
)

# Job title keywords to identify experience sections
_ROLE_INDICATORS = re.compile(
    r"\b(engineer|developer|analyst|manager|director|lead|architect|consultant|"
    r"specialist|designer|scientist|intern|associate|senior|junior|principal|"
    r"staff|vp|cto|ceo|cfo|head of|officer)\b",
    re.IGNORECASE,
)


def _extract_experience(text: str) -> list[ParsedExperience]:
    """
    Best-effort work experience extraction.
    Looks for blocks that contain a date range near a job-title indicator.
    """
    entries: list[ParsedExperience] = []
    lines = [ln.strip() for ln in text.splitlines() if ln.strip()]

    i = 0
    while i < len(lines):
        line = lines[i]
        date_match = _DATE_RANGE_RE.search(line)
        if date_match:
            date_range = date_match.group(0).strip()
            # Look ±2 lines for a role/company
            context_lines = lines[max(0, i - 2): i + 3]
            role_line = next(
                (ln for ln in context_lines if _ROLE_INDICATORS.search(ln) and ln != line),
                None,
            )
            company_line = next(
                (ln for ln in context_lines if ln != line and ln != role_line and len(ln) > 2),
                None,
            )
            # Tidy up: strip trailing bullet / punctuation from role/company
            role = re.sub(r"[|·•\-–—]", " ", role_line or "").strip() if role_line else None
            company = re.sub(r"[|·•\-–—]", " ", company_line or "").strip() if company_line else None

            if role or company:
                entries.append(ParsedExperience(
                    role=role or "",
                    company=company or "",
                    duration=date_range,
                ))
            i += 2
        else:
            i += 1

    # Deduplicate by (role, company)
    seen: set[tuple[str, str]] = set()
    unique: list[ParsedExperience] = []
    for e in entries:
        key = (e.role.lower()[:40], e.company.lower()[:40])
        if key not in seen:
            seen.add(key)
            unique.append(e)
    return unique[:8]  # Cap at 8 entries


# ── Education extraction ──────────────────────────────────────────────────────

_DEGREE_PATTERNS = [
    (re.compile(r"\bph\.?d\.?\b|\bdoctor(?:ate)?\b", re.I), "PhD"),
    (re.compile(r"\bm\.?tech\.?\b|\bmaster(?:s)?\s+(?:of\s+)?(?:science|engineering|technology|arts|business)?\b|\bmba\b|\bm\.s\.?\b|\bm\.e\.?\b|\bm\.a\.?\b", re.I), "Master's"),
    (re.compile(r"\bb\.?tech\.?\b|\bb\.?e\.?\b|\bb\.?sc\.?\b|\bb\.?s\.?\b|\bb\.?a\.?\b|\bbachelor(?:s)?\s+(?:of\s+)?(?:science|engineering|technology|arts)?\b", re.I), "Bachelor's"),
    (re.compile(r"\bassociate(?:s)?\b|\bdiploma\b", re.I), "Associate/Diploma"),
]

_YEAR_RE = re.compile(r"\b(19|20)\d{2}\b")
_INSTITUTION_KEYWORDS = re.compile(
    r"\b(?:university|college|institute|school|academy|iit|nit|bits|mit|stanford|"
    r"harvard|oxford|cambridge)\b",
    re.IGNORECASE,
)


def _extract_education(text: str) -> list[ParsedEducation]:
    entries: list[ParsedEducation] = []
    lines = [ln.strip() for ln in text.splitlines() if ln.strip()]

    for i, line in enumerate(lines):
        degree_label = None
        for pattern, label in _DEGREE_PATTERNS:
            if pattern.search(line):
                degree_label = label
                break
        if not degree_label:
            continue

        # Find year in this line or adjacent
        year_match = _YEAR_RE.search(line)
        if not year_match:
            for adj in lines[max(0, i - 1): i + 2]:
                year_match = _YEAR_RE.search(adj)
                if year_match:
                    break
        year = year_match.group(0) if year_match else None

        # Find institution name
        institution = None
        for adj in lines[max(0, i - 2): i + 3]:
            if _INSTITUTION_KEYWORDS.search(adj) and adj != line:
                institution = re.sub(r"[|·•\-–—]", " ", adj).strip()
                break
        if not institution and _INSTITUTION_KEYWORDS.search(line):
            institution = re.sub(r"[|·•\-–—]", " ", line).strip()

        # Extract field from the degree line
        field_match = re.search(
            r"(?:in|of)\s+([A-Za-z][A-Za-z\s&]{2,40})(?:\s*,|\s*\(|\s*\d|$)",
            line,
        )
        field = field_match.group(1).strip() if field_match else None

        entries.append(ParsedEducation(
            degree=degree_label,
            field=field,
            institution=institution,
            year=year,
        ))

    # Deduplicate
    seen: set[str] = set()
    unique: list[ParsedEducation] = []
    for e in entries:
        key = f"{e.degree}{e.institution or ''}{e.year or ''}"
        if key not in seen:
            seen.add(key)
            unique.append(e)
    return unique[:4]


# ── Summary extraction ────────────────────────────────────────────────────────

_SUMMARY_HEADERS = re.compile(
    r"^(?:summary|profile|objective|about|overview|professional summary|"
    r"career objective|personal statement)[\s:–—]*$",
    re.IGNORECASE,
)


def _extract_summary(text: str) -> str:
    """Extract professional summary section, or fall back to first substantive paragraph."""
    lines = [ln.strip() for ln in text.splitlines()]
    # Look for an explicit summary header
    for i, line in enumerate(lines):
        if _SUMMARY_HEADERS.match(line):
            # Collect next non-empty lines until blank line or next header
            summary_lines: list[str] = []
            for j in range(i + 1, min(i + 8, len(lines))):
                if not lines[j]:
                    break
                if len(lines[j]) < 15:  # likely a new header
                    break
                summary_lines.append(lines[j])
            if summary_lines:
                return " ".join(summary_lines)[:500]

    # Fallback: first paragraph with 40+ chars that looks like prose
    paragraph: list[str] = []
    for line in lines:
        if len(line) >= 40 and not line.isupper():
            paragraph.append(line)
            if len(" ".join(paragraph)) >= 120:
                break
        elif paragraph:
            break

    return " ".join(paragraph)[:500] if paragraph else ""


# ── Route ─────────────────────────────────────────────────────────────────────

@router.post("/parse", response_model=ParseResumeResponse, tags=["Parsing"])
async def parse_resume(request: ParseResumeRequest) -> ParseResumeResponse:
    """
    Extract structured sections from raw resume text.

    Returns extracted skills, work experience entries, education entries,
    and a professional summary. No external API — fully local pattern-based.
    """
    skills = _extract_skills(request.resume_text)
    experience = _extract_experience(request.resume_text)
    education = _extract_education(request.resume_text)
    summary = _extract_summary(request.resume_text)

    return ParseResumeResponse(
        candidate_id=request.candidate_id,
        skills=skills,
        experience=experience,
        education=education,
        summary=summary,
        parsed_at=datetime.now(timezone.utc).isoformat(),
    )
