using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Queries.GetKanban;

public class GetKanbanHandler : IRequestHandler<GetKanbanQuery, IReadOnlyList<JobApplicationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetKanbanHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<JobApplicationDto>> Handle(GetKanbanQuery request, CancellationToken ct)
    {
        var query = _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Candidate)
            .Include(a => a.JobPosting)
            .Where(a => !a.IsDeleted);

        if (request.JobPostingId.HasValue)
            query = query.Where(a => a.JobPostingId == request.JobPostingId.Value);

        var applications = await query
            .OrderByDescending(a => a.MatchScore)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

        return applications.Select(a => a.ToDto()).ToList();
    }
}
