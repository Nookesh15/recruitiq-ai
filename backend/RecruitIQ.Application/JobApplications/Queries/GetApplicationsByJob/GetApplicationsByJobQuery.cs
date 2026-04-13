using MediatR;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Queries.GetApplicationsByJob;

public record GetApplicationsByJobQuery(Guid JobPostingId) : IRequest<IReadOnlyList<JobApplicationDto>>;
