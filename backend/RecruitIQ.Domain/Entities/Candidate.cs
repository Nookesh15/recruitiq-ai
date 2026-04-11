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

    public ICollection<JobApplication> Applications { get; set; } = [];
}
