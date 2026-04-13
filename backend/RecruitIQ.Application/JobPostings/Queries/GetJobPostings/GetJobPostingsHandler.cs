using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;

namespace RecruitIQ.Application.JobPostings.Queries.GetJobPostings;

public class GetJobPostingsHandler : IRequestHandler<GetJobPostingsQuery, PaginatedList<JobPostingDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetJobPostingsHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<JobPostingDto>> Handle(GetJobPostingsQuery request, CancellationToken ct)
    {
        var query = _context.JobPostings
            .AsNoTracking()
            .OrderByDescending(j => j.CreatedAt);

        // Recruiters only see job postings they created
        var filtered = _currentUser.IsAdmin
            ? query.AsQueryable()
            : query.Where(j => j.CreatedById == _currentUser.UserId || j.CreatedById == null);

        var total = await filtered.CountAsync(ct);

        var items = await filtered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(j => new JobPostingDto(
                j.Id, j.Title, j.Description, j.Department,
                j.Location, j.Status,
                j.Applications.Count(a => !a.IsDeleted),
                j.CreatedAt))
            .ToListAsync(ct);

        return new PaginatedList<JobPostingDto>(items, total, request.Page, request.PageSize);
    }
}
