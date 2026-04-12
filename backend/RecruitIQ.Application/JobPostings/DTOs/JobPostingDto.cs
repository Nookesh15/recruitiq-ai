using RecruitIQ.Domain.Enums;

namespace RecruitIQ.Application.JobPostings.DTOs;

public record JobPostingDto(
    Guid Id,
    string Title,
    string Description,
    string Department,
    string Location,
    JobStatus Status,
    int ApplicationCount,
    DateTime CreatedAt
);
