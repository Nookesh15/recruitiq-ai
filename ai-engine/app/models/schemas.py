from pydantic import BaseModel, Field


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


class HealthResponse(BaseModel):
    status: str
    version: str
    model_loaded: bool
