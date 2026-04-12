using MediatR;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;

namespace RecruitIQ.Application.JobPostings.Queries.GetJobPostings;

public record GetJobPostingsQuery(int Page = 1, int PageSize = 20) : IRequest<PaginatedList<JobPostingDto>>;
