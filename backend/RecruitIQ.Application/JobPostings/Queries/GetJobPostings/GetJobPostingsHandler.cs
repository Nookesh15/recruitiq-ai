using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;

namespace RecruitIQ.Application.JobPostings.Queries.GetJobPostings;

public class GetJobPostingsHandler : IRequestHandler<GetJobPostingsQuery, PaginatedList<JobPostingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetJobPostingsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<JobPostingDto>> Handle(GetJobPostingsQuery request, CancellationToken ct)
    {
        var query = _context.JobPostings
            .Include(j => j.Applications)
            .OrderByDescending(j => j.CreatedAt);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(j => new JobPostingDto(
                j.Id, j.Title, j.Description, j.Department,
                j.Location, j.Status, j.Applications.Count, j.CreatedAt))
            .ToListAsync(ct);

        return new PaginatedList<JobPostingDto>(items, total, request.Page, request.PageSize);
    }
}
