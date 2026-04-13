using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecruitIQ.API.Services;
using RecruitIQ.Application.Candidates.Commands.CreateCandidate;
using RecruitIQ.Application.Candidates.Commands.DeleteCandidate;
using RecruitIQ.Application.Candidates.Commands.UpdateCandidateStatus;
using RecruitIQ.Application.Candidates.Commands.UploadResume;
using RecruitIQ.Application.Candidates.Queries.GetCandidateById;
using RecruitIQ.Application.Candidates.Queries.GetCandidates;

namespace RecruitIQ.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CandidatesController> _logger;

    public CandidatesController(IMediator mediator, ILogger<CandidatesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCandidatesQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCandidateByIdQuery(id), ct);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCandidateCommand command,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequest body,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new UpdateCandidateStatusCommand(id, body.Status), ct);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new DeleteCandidateCommand(id), ct);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return NoContent();
    }

    [HttpPost("{id:guid}/resume")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> UploadResume(
        Guid id,
        IFormFile file,
        CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        var allowedExts = new[] { ".pdf", ".doc", ".docx" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExts.Contains(ext))
            return BadRequest(new { error = "Only PDF, DOC, DOCX files are accepted." });

        // Save file
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes");
        Directory.CreateDirectory(uploadsPath);
        var fileName = $"{id}{ext}";
        var filePath = Path.Combine(uploadsPath, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream, ct);

        var resumeUrl = $"/resumes/{fileName}";

        // Extract text so the AI scoring + parsing pipeline fires
        var resumeText = ResumeTextExtractor.Extract(filePath, ext, _logger);

        var result = await _mediator.Send(new UploadResumeCommand(id, resumeUrl, resumeText), ct);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }
}

public record UpdateStatusRequest(RecruitIQ.Domain.Enums.CandidateStatus Status);
