using MediatR;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;

namespace RecruitIQ.Application.JobPostings.Queries.GetJobPostingById;

public record GetJobPostingByIdQuery(Guid Id) : IRequest<Result<JobPostingDto>>;
