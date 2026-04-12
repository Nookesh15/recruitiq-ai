using MediatR;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;

namespace RecruitIQ.Application.JobPostings.Commands.CreateJobPosting;

public record CreateJobPostingCommand(
    string Title,
    string Description,
    string Department,
    string Location
) : IRequest<Result<JobPostingDto>>;
