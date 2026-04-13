using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;

namespace RecruitIQ.Application.Analytics.Queries;

public class GetFunnelHandler : IRequestHandler<GetFunnelQuery, FunnelResult>
{
    private static readonly string[] StageOrder =
        ["Applied", "Screening", "Interview", "Offer", "Hired", "Rejected"];

    private readonly IApplicationDbContext _context;

    public GetFunnelHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FunnelResult> Handle(GetFunnelQuery request, CancellationToken ct)
    {
        var query = _context.JobApplications
            .AsNoTracking()
            .Where(a => !a.IsDeleted);

        if (request.JobPostingId.HasValue)
            query = query.Where(a => a.JobPostingId == request.JobPostingId.Value);

        var all = await query
            .Select(a => new { a.Stage, a.MatchScore, a.CreatedAt, a.UpdatedAt })
            .ToListAsync(ct);

        var total = all.Count;

        // Stage counts
        var stageCounts = StageOrder.Select(stage => new
        {
            Stage = stage,
            Count = all.Count(a => a.Stage == stage),
        }).ToList();

        // Drop-off % relative to previous forward stage (Rejected is separate)
        var forwardStages = stageCounts.Where(s => s.Stage != "Rejected").ToList();
        var funnel = StageOrder.Select(stage =>
        {
            var count = stageCounts.First(s => s.Stage == stage).Count;
            double dropOff = 0;
            if (stage != "Applied" && stage != "Rejected" && total > 0)
            {
                var prevIdx = Array.IndexOf(StageOrder, stage) - 1;
                var prevStage = StageOrder[prevIdx];
                // Only compute for non-rejected flow
                var prevCount = forwardStages.FirstOrDefault(s => s.Stage == prevStage)?.Count ?? 0;
                dropOff = prevCount > 0 ? Math.Round((1.0 - (double)count / prevCount) * 100, 1) : 0;
            }
            return new StageFunnelItem(stage, count, dropOff);
        }).ToList();

        // Avg AI score
        var scored = all.Where(a => a.MatchScore.HasValue).Select(a => a.MatchScore!.Value).ToList();
        double? avgScore = scored.Count > 0 ? Math.Round(scored.Average(), 1) : null;

        // Avg days to hire (applied → hired, use UpdatedAt as proxy for stage change)
        var hired = all.Where(a => a.Stage == "Hired").ToList();
        double? avgDaysToHire = hired.Count > 0
            ? Math.Round(hired.Average(a => (a.UpdatedAt - a.CreatedAt).TotalDays), 1)
            : null;

        return new FunnelResult(funnel, total, avgScore, avgDaysToHire);
    }
}
