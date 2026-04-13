using RecruitIQ.Domain.Enums;

namespace RecruitIQ.Domain.Entities;

public class Candidate : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ResumeUrl { get; set; }
    public CandidateStatus Status { get; set; } = CandidateStatus.Applied;
    public double? AiScore { get; set; }

    // Structured resume sections populated by AI engine (RIQAI-18)
    public string? ParsedSkillsJson { get; set; }      // JSON array of skill strings
    public string? ParsedExperienceJson { get; set; }  // JSON array of {role, company, duration}
    public string? ParsedEducationJson { get; set; }   // JSON array of {degree, field, institution, year}
    public string? ParsedSummary { get; set; }

    // RIQAI-26: row-level ownership for Recruiter filtering
    public Guid? CreatedById { get; set; }

    public ICollection<JobApplication> Applications { get; set; } = [];
}
