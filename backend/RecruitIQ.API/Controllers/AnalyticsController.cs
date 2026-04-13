using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitIQ.Application.Analytics.Queries;

namespace RecruitIQ.API.Controllers;

[ApiController]
[Route("api/v1/analytics")]
[Authorize]
public class AnalyticsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns hiring funnel breakdown: stage counts, drop-off %, avg AI score, avg days-to-hire.
    /// Pass ?jobId= to filter to a specific posting.
    /// </summary>
    [HttpGet("funnel")]
    public async Task<IActionResult> GetFunnel([FromQuery] Guid? jobId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFunnelQuery(jobId), ct);
        return Ok(result);
    }
}
