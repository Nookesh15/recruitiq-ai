namespace RecruitIQ.Application.JobPostings.DTOs;

public record PublicJobDto(
    Guid Id,
    string Title,
    string Description,
    string Department,
    string Location,
    string Status,
    DateTime CreatedAt
);
