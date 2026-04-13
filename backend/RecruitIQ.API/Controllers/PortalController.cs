using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitIQ.Application.JobApplications.Commands.ApplyToJob;
using RecruitIQ.Application.JobPostings.Queries.GetPublicJobDetails;

namespace RecruitIQ.API.Controllers;

/// <summary>
/// Public job portal — no authentication required.
/// </summary>
[ApiController]
[Route("api/v1/portal")]
[AllowAnonymous]
public class PortalController : ControllerBase
{
    private readonly IMediator _mediator;

    public PortalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>GET public job details for the apply page.</summary>
    [HttpGet("jobs/{jobId:guid}")]
    public async Task<IActionResult> GetJob(Guid jobId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPublicJobDetailsQuery(jobId), ct);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Submit a job application from the public portal.
    /// Accepts multipart/form-data with candidate info + resume file.
    /// </summary>
    [HttpPost("jobs/{jobId:guid}/apply")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> Apply(
        Guid jobId,
        [FromForm] PortalApplyRequest form,
        IFormFile? resume,
        CancellationToken ct = default)
    {
        if (resume is null || resume.Length == 0)
            return BadRequest(new { error = "Resume file is required." });

        var allowedExts = new[] { ".pdf", ".doc", ".docx" };
        var ext = Path.GetExtension(resume.FileName).ToLowerInvariant();
        if (!allowedExts.Contains(ext))
            return BadRequest(new { error = "Only PDF, DOC, DOCX files are accepted." });

        // Save resume file
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes");
        Directory.CreateDirectory(uploadsPath);
        var fileName = $"{jobId}_{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsPath, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await resume.CopyToAsync(stream, ct);

        var resumeUrl = $"/resumes/{fileName}";
        var resumeText = string.Empty; // Future: extract text from PDF

        var result = await _mediator.Send(new ApplyToJobCommand(
            jobId,
            form.FirstName,
            form.LastName,
            form.Email,
            form.Phone,
            resumeUrl,
            resumeText), ct);

        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}

public record PortalApplyRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? CoverNote
);
