namespace RecruitIQ.Domain.Entities;

public class JobApplication : BaseEntity
{
    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public Guid JobPostingId { get; set; }
    public JobPosting JobPosting { get; set; } = null!;

    public double? MatchScore { get; set; }
    public string? Notes { get; set; }
    public string Stage { get; set; } = "Applied"; // Applied | Screening | Interview | Offer | Hired | Rejected

    // RIQAI-19: AI match reasoning
    public string? MatchReason { get; set; }
    public string? StrengthsJson { get; set; }  // JSON array of strength strings
    public string? GapsJson { get; set; }        // JSON array of gap strings
}
