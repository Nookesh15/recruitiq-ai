using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Queries.GetCandidates;

public class GetCandidatesHandler : IRequestHandler<GetCandidatesQuery, PaginatedList<CandidateDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCandidatesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<CandidateDto>> Handle(GetCandidatesQuery request, CancellationToken ct)
    {
        var query = _context.Candidates
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CandidateDto(
                c.Id, c.FirstName, c.LastName, c.Email,
                c.Phone, c.ResumeUrl, c.Status, c.AiScore, c.CreatedAt))
            .ToListAsync(ct);

        return new PaginatedList<CandidateDto>(items, total, request.Page, request.PageSize);
    }
}
