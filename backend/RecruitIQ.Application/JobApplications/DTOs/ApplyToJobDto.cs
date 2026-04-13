namespace RecruitIQ.Application.JobApplications.DTOs;

public record ApplyToJobDto(
    Guid ApplicationId,
    Guid CandidateId,
    string CandidateName,
    string Email,
    Guid JobPostingId,
    string JobTitle,
    int? AiScore,
    string Message
);
