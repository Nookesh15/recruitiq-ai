using MediatR;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Commands.CreateJobApplication;

public record CreateJobApplicationCommand(
    Guid CandidateId,
    Guid JobPostingId,
    string? Notes
) : IRequest<Result<JobApplicationDto>>;
