using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitIQ.Application.JobApplications.Commands.CreateJobApplication;
using RecruitIQ.Application.JobApplications.Commands.UpdateStage;
using RecruitIQ.Application.JobApplications.Queries.GetApplicationsByCandidate;
using RecruitIQ.Application.JobApplications.Queries.GetApplicationsByJob;
using RecruitIQ.Application.JobApplications.Queries.GetKanban;

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

    /// <summary>Returns all applications for the Kanban board, optionally filtered by job.</summary>
    [HttpGet("kanban")]
    public async Task<IActionResult> GetKanban([FromQuery] Guid? jobId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetKanbanQuery(jobId), ct);
        return Ok(result);
    }

    /// <summary>Move an application to a different pipeline stage.</summary>
    [HttpPatch("{id:guid}/stage")]
    public async Task<IActionResult> UpdateStage(
        Guid id,
        [FromBody] UpdateStageRequest request,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new UpdateStageCommand(id, request.Stage), ct);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    public record UpdateStageRequest(string Stage);
}
