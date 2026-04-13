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
        return await _context.JobApplications
            .AsNoTracking()
            .Where(a => a.JobPostingId == request.JobPostingId)
            .OrderByDescending(a => a.MatchScore)
            .ThenByDescending(a => a.CreatedAt)
            .Select(a => new JobApplicationDto(
                a.Id,
                a.Candidate.Id,
                $"{a.Candidate.FirstName} {a.Candidate.LastName}",
                a.Candidate.Email,
                a.JobPosting.Id,
                a.JobPosting.Title,
                a.MatchScore,
                a.Notes,
                a.CreatedAt))
            .ToListAsync(ct);
    }
}
