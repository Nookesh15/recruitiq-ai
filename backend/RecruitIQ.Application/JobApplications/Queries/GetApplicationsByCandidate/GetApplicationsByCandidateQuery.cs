using MediatR;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Queries.GetApplicationsByCandidate;

public record GetApplicationsByCandidateQuery(Guid CandidateId) : IRequest<IReadOnlyList<JobApplicationDto>>;
