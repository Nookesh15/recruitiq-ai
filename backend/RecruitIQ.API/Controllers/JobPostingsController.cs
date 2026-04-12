using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecruitIQ.Application.JobPostings.Commands.CreateJobPosting;
using RecruitIQ.Application.JobPostings.Commands.UpdateJobPostingStatus;
using RecruitIQ.Application.JobPostings.Queries.GetJobPostingById;
using RecruitIQ.Application.JobPostings.Queries.GetJobPostings;

namespace RecruitIQ.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class JobPostingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobPostingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetJobPostingsQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetJobPostingByIdQuery(id), ct);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateJobPostingCommand command,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateJobStatusRequest body,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new UpdateJobPostingStatusCommand(id, body.Status), ct);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }
}

public record UpdateJobStatusRequest(RecruitIQ.Domain.Enums.JobStatus Status);
