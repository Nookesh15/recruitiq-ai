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
        return await _context.JobApplications
            .Include(a => a.Candidate)
            .Include(a => a.JobPosting)
            .Where(a => a.CandidateId == request.CandidateId)
            .OrderByDescending(a => a.CreatedAt)
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
