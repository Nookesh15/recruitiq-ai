using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Queries.GetApplicationsByJob;

public class GetApplicationsByJobHandler
    : IRequestHandler<GetApplicationsByJobQuery, IReadOnlyList<JobApplicationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetApplicationsByJobHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<JobApplicationDto>> Handle(
        GetApplicationsByJobQuery request, CancellationToken ct)
    {
        var applications = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Candidate)
            .Include(a => a.JobPosting)
            .Where(a => a.JobPostingId == request.JobPostingId)
            .OrderByDescending(a => a.MatchScore)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

        return applications.Select(a => a.ToDto()).ToList();
    }
}
