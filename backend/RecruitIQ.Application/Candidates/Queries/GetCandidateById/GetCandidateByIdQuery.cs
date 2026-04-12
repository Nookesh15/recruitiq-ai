using MediatR;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Queries.GetCandidateById;

public record GetCandidateByIdQuery(Guid Id) : IRequest<Result<CandidateDto>>;
