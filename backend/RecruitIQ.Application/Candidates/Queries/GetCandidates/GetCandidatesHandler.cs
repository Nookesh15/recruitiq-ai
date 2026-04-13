using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Queries.GetCandidates;

public class GetCandidatesHandler : IRequestHandler<GetCandidatesQuery, PaginatedList<CandidateDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCandidatesHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<CandidateDto>> Handle(GetCandidatesQuery request, CancellationToken ct)
    {
        var baseQuery = _context.Candidates
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt);

        // Recruiters only see candidates they created
        var filtered = _currentUser.IsAdmin
            ? baseQuery
            : baseQuery.Where(c => c.CreatedById == _currentUser.UserId || c.CreatedById == null);

        var total = await filtered.CountAsync(ct);

        var rawItems = await filtered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var items = rawItems.Select(c => c.ToDto()).ToList();

        return new PaginatedList<CandidateDto>(items, total, request.Page, request.PageSize);
    }
}
