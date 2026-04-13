namespace RecruitIQ.Application.JobApplications.DTOs;

public record JobApplicationDto(
    Guid Id,
    Guid CandidateId,
    string CandidateName,
    string CandidateEmail,
    Guid JobPostingId,
    string JobTitle,
    double? MatchScore,
    string? Notes,
    DateTime CreatedAt,
    // RIQAI-19: AI match reasoning
    string? MatchReason,
    List<string>? Strengths,
    List<string>? Gaps,
    // RIQAI-23: Kanban stage
    string Stage = "Applied"
);
