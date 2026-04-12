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
    DateTime CreatedAt
);
