using MediatR;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;
using RecruitIQ.Domain.Enums;

namespace RecruitIQ.Application.JobPostings.Commands.UpdateJobPostingStatus;

public record UpdateJobPostingStatusCommand(Guid Id, JobStatus Status) : IRequest<Result<JobPostingDto>>;
