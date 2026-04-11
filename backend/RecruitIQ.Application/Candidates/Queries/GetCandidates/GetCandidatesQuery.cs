using MediatR;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Queries.GetCandidates;

public record GetCandidatesQuery(int Page = 1, int PageSize = 20) : IRequest<PaginatedList<CandidateDto>>;
