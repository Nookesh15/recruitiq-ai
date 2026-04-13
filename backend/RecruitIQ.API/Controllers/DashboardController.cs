using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Domain.Enums;

namespace RecruitIQ.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public DashboardController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken ct = default)
    {
        var totalCandidates = await _context.Candidates.CountAsync(ct);

        var activeJobs = await _context.JobPostings
            .CountAsync(j => j.Status == JobStatus.Active, ct);

        var totalApplications = await _context.JobApplications.CountAsync(ct);

        var scored = await _context.Candidates
            .Where(c => c.AiScore != null)
            .Select(c => c.AiScore!.Value)
            .ToListAsync(ct);

        var avgScore = scored.Count > 0 ? (int)Math.Round(scored.Average()) : 0;

        var pipeline = await _context.Candidates
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(ct);

        var recentApplications = await _context.JobApplications
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .Select(a => new
            {
                a.Id,
                CandidateName = $"{a.Candidate.FirstName} {a.Candidate.LastName}",
                a.Candidate.Email,
                JobTitle = a.JobPosting.Title,
                a.MatchScore,
                a.CreatedAt
            })
            .ToListAsync(ct);

        return Ok(new
        {
            totalCandidates,
            activeJobs,
            totalApplications,
            avgAiScore = scored.Count > 0 ? (int?)avgScore : null,
            scoredCount = scored.Count,
            pipeline,
            recentApplications
        });
    }
}
