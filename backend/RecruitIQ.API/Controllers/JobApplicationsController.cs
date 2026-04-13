using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecruitIQ.Application.JobApplications.Commands.CreateJobApplication;
using RecruitIQ.Application.JobApplications.Queries.GetApplicationsByCandidate;
using RecruitIQ.Application.JobApplications.Queries.GetApplicationsByJob;

namespace RecruitIQ.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class JobApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("candidate/{candidateId:guid}")]
    public async Task<IActionResult> GetByCandidate(Guid candidateId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetApplicationsByCandidateQuery(candidateId), ct);
        return Ok(result);
    }

    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetByJob(Guid jobId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetApplicationsByJobQuery(jobId), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateJobApplicationCommand command,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Created(string.Empty, result.Value);
    }
}
