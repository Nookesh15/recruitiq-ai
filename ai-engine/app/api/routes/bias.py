"""
RIQAI-20: Bias detection on job description text.
Scans JD for language patterns associated with gender-coding, age bias,
ableism, and other exclusionary phrasing. Fully local — no external API.
"""
from __future__ import annotations

import re

from fastapi import APIRouter

from app.models.schemas import AnalyzeJdRequest, AnalyzeJdResponse, BiasFlag

router = APIRouter()

# ── Bias rule definitions ─────────────────────────────────────────────────────
# Each rule: (regex_pattern, category, suggestion)

_RULES: list[tuple[re.Pattern[str], str, str]] = [
    # Gender-coded — masculine
    (re.compile(r"\b(ninja|rockstar|rock star|guru|wizard|hero|champion|warrior|badass|bro)\b", re.I),
     "Gender-coded (masculine)", "Use neutral terms like 'expert', 'skilled', 'experienced'"),

    (re.compile(r"\b(aggressive(?:ly)?|dominant|dominate|compet(?:e|itive|ition)|strong personality|assertive)\b", re.I),
     "Gender-coded (masculine)", "Replace with specific behavioural traits e.g. 'goal-oriented', 'results-driven'"),

    # Gender-coded — feminine (can discourage male applicants too)
    (re.compile(r"\b(nurtur(?:e|ing)|support(?:ive)?|collaborat(?:e|ive)|interpersonal|empath(?:y|etic)|compassion(?:ate)?)\b", re.I),
     "Gender-coded (feminine)", "Pair with technical or results-based language to avoid gendered signal"),

    # Age bias
    (re.compile(r"\b(recent graduate|new graduate|fresh graduate|young professional|young talent|digital native)\b", re.I),
     "Age bias", "Remove age-implied terms; describe skills or experience level instead"),

    (re.compile(r"\b(junior|entry.?level)\b.{0,30}?\b(energetic|dynamic|fast.?paced)\b", re.I),
     "Age bias", "Avoid combining seniority level with age-coded energy descriptors"),

    (re.compile(r"\b(minimum|at least|no more than)\s+\d+\s+years?\b", re.I),
     "Age bias", "Years-of-experience requirements can be age-discriminatory; consider describing proficiency level instead"),

    # Ableist language
    (re.compile(r"\b(crazy|insane|mental(?:ly)?|psycho|OCD|blind to|tone.?deaf|lame|cripple|wheelchair)\b", re.I),
     "Ableist language", "Replace with specific, literal descriptions of the requirement or trait"),

    (re.compile(r"\b(able.bodied|physically fit|physically demanding|must be able to lift|stand for long)\b", re.I),
     "Ableist language", "Only include physical requirements if genuinely essential to the role"),

    # Exclusionary / gatekeeping
    (re.compile(r"\b(native speaker|mother tongue|born.?in|citizenship required)\b", re.I),
     "Exclusionary language", "Specify required language proficiency level (e.g. C1/fluent) rather than origin"),

    (re.compile(r"\b(culture fit|culture match|fit our culture)\b", re.I),
     "Exclusionary language", "Replace 'culture fit' with specific values or working-style traits to avoid homogeneity bias"),

    (re.compile(r"\b(he[/ ]?she|his[/ ]?her|guys|mankind)\b", re.I),
     "Gendered pronouns", "Use gender-neutral pronouns: 'they/them', 'the candidate', 'team members'"),

    # Unnecessary requirements
    (re.compile(r"\b(must be (a )?perfectionist|detail.?oriented to a fault|type.?a personality)\b", re.I),
     "Unnecessary trait requirement", "Describe specific quality standards instead of personality archetypes"),

    (re.compile(r"\b(work hard play hard|hustle|grind|always on|24[/ ]7|on.?call always)\b", re.I),
     "Work-life balance signal", "Avoid phrases that signal poor boundaries; describe on-call expectations concretely if needed"),
]


def _detect_flags(text: str) -> list[BiasFlag]:
    """Run all bias rules against the JD text. Deduplicate by phrase."""
    found: dict[str, BiasFlag] = {}  # key = lower-case phrase to deduplicate
    for pattern, category, suggestion in _RULES:
        for match in pattern.finditer(text):
            phrase = match.group(0)
            key = phrase.lower()
            if key not in found:
                found[key] = BiasFlag(phrase=phrase, category=category, suggestion=suggestion)
    return list(found.values())


# ── Route ─────────────────────────────────────────────────────────────────────

@router.post("/analyze-jd", response_model=AnalyzeJdResponse, tags=["Bias Detection"])
async def analyze_jd(request: AnalyzeJdRequest) -> AnalyzeJdResponse:
    """
    Scan a job description for potentially biased or exclusionary language.

    Returns a list of flagged phrases with category and suggested rewording.
    An empty list means no issues were detected.
    """
    flags = _detect_flags(request.jd_text)
    return AnalyzeJdResponse(
        flag_count=len(flags),
        bias_flags=flags,
        is_clean=len(flags) == 0,
    )
