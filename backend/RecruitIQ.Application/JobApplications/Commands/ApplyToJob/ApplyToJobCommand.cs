using MediatR;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Commands.ApplyToJob;

public record ApplyToJobCommand(
    Guid JobPostingId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string ResumeUrl,
    string ResumeText
) : IRequest<Result<ApplyToJobDto>>;
