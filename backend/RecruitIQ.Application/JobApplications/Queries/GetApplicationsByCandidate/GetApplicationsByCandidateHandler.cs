using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Queries.GetApplicationsByCandidate;

public class GetApplicationsByCandidateHandler
    : IRequestHandler<GetApplicationsByCandidateQuery, IReadOnlyList<JobApplicationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetApplicationsByCandidateHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<JobApplicationDto>> Handle(
        GetApplicationsByCandidateQuery request, CancellationToken ct)
    {
        var applications = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Candidate)
            .Include(a => a.JobPosting)
            .Where(a => a.CandidateId == request.CandidateId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

        return applications.Select(a => a.ToDto()).ToList();
    }
}
