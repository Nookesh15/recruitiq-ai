using MediatR;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;

namespace RecruitIQ.Application.JobPostings.Queries.GetPublicJobDetails;

public record GetPublicJobDetailsQuery(Guid JobId) : IRequest<Result<PublicJobDto>>;
