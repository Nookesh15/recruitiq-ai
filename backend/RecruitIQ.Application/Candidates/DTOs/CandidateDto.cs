using RecruitIQ.Domain.Enums;

namespace RecruitIQ.Application.Candidates.DTOs;

public record CandidateDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? ResumeUrl,
    CandidateStatus Status,
    double? AiScore,
    DateTime CreatedAt
);
