from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=".env", extra="ignore")

    app_name: str = "RecruitIQ AI Engine"
    app_version: str = "0.1.0"
    debug: bool = False

    # SQL Server connection (used by .NET backend; AI engine reads via REST)
    database_url: str = ""

    # Internal API key for service-to-service calls
    api_key: str = "dev-secret-key"

    # Scoring weights
    skill_match_weight: float = 0.5
    experience_weight: float = 0.3
    education_weight: float = 0.2


settings = Settings()
