from pydantic import BaseModel, Field


# ── Scoring schemas ───────────────────────────────────────────────────────────

class ResumeScoreRequest(BaseModel):
    candidate_id: str = Field(..., description="UUID of the candidate")
    resume_text: str = Field(..., min_length=10, description="Raw resume text")
    job_description: str = Field(..., min_length=10, description="Job description to match against")
    required_skills: list[str] = Field(default_factory=list, description="Required skills for the role")


class ResumeScoreResponse(BaseModel):
    candidate_id: str
    overall_score: float = Field(..., ge=0, le=100, description="AI score 0-100")
    skill_match_score: float = Field(..., ge=0, le=100)
    experience_score: float = Field(..., ge=0, le=100)
    education_score: float = Field(..., ge=0, le=100)
    matched_skills: list[str]
    missing_skills: list[str]
    summary: str
    # RIQAI-19: match reasoning
    match_reason: str = Field("", description="2-3 sentence human-readable fit explanation")
    strengths: list[str] = Field(default_factory=list, description="Key candidate strengths for this role")
    gaps: list[str] = Field(default_factory=list, description="Notable gaps or missing requirements")


# ── Parsing schemas ───────────────────────────────────────────────────────────

class ParsedExperience(BaseModel):
    role: str = Field(..., description="Job title / role")
    company: str = Field(..., description="Employer name")
    duration: str = Field(..., description="Date range e.g. 'Jan 2020 – Mar 2023'")


class ParsedEducation(BaseModel):
    degree: str = Field(..., description="Degree level e.g. Bachelor's, Master's, PhD")
    field: str | None = Field(None, description="Field of study e.g. Computer Science")
    institution: str | None = Field(None, description="University / college name")
    year: str | None = Field(None, description="Graduation year")


class ParseResumeRequest(BaseModel):
    candidate_id: str = Field(..., description="UUID of the candidate")
    resume_text: str = Field(..., min_length=10, description="Raw resume text to parse")


class ParseResumeResponse(BaseModel):
    candidate_id: str
    skills: list[str] = Field(default_factory=list, description="Detected skills and technologies")
    experience: list[ParsedExperience] = Field(default_factory=list, description="Work experience entries")
    education: list[ParsedEducation] = Field(default_factory=list, description="Education entries")
    summary: str = Field("", description="Professional summary extracted from resume")
    parsed_at: str = Field(..., description="ISO 8601 timestamp of when parsing ran")


# ── Health schema ─────────────────────────────────────────────────────────────

class HealthResponse(BaseModel):
    status: str
    version: str
    model_loaded: bool
