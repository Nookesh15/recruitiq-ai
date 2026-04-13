using MediatR;

namespace RecruitIQ.Application.Analytics.Queries;

public record StageFunnelItem(string Stage, int Count, double DropOffPct);

public record FunnelResult(
    List<StageFunnelItem> Funnel,
    int Total,
    double? AvgScore,
    double? AvgDaysToHire        // null if no one has reached Hired yet
);

/// <param name="JobPostingId">Optional — filter to a single job</param>
public record GetFunnelQuery(Guid? JobPostingId) : IRequest<FunnelResult>;
