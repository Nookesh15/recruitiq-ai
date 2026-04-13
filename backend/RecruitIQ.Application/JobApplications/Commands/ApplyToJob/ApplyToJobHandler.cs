using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobApplications.DTOs;
using RecruitIQ.Domain.Entities;
using RecruitIQ.Domain.Enums;

namespace RecruitIQ.Application.JobApplications.Commands.ApplyToJob;

public class ApplyToJobHandler : IRequestHandler<ApplyToJobCommand, Result<ApplyToJobDto>>
{
    private static readonly JsonSerializerOptions _opts = new(JsonSerializerDefaults.Web);
    private readonly IApplicationDbContext _context;
    private readonly IAiEngineService _aiEngine;

    public ApplyToJobHandler(IApplicationDbContext context, IAiEngineService aiEngine)
    {
        _context = context;
        _aiEngine = aiEngine;
    }

    public async Task<Result<ApplyToJobDto>> Handle(ApplyToJobCommand request, CancellationToken ct)
    {
        // Validate job exists and is active
        var job = await _context.JobPostings
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == request.JobPostingId, ct);

        if (job is null)
            return Result<ApplyToJobDto>.Failure("Job posting not found.");

        if (job.Status != JobStatus.Active)
            return Result<ApplyToJobDto>.Failure("This job posting is not currently accepting applications.");

        // Find existing candidate by email or create new one
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Email == request.Email, ct);

        if (candidate is null)
        {
            candidate = new Candidate
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Status = CandidateStatus.Applied,
                ResumeUrl = request.ResumeUrl
            };
            await _context.Candidates.AddAsync(candidate, ct);
            await _context.SaveChangesAsync(ct);
        }
        else
        {
            // Update resume if re-applying
            candidate.ResumeUrl = request.ResumeUrl;
        }

        // Check for duplicate application
        var alreadyApplied = await _context.JobApplications
            .AnyAsync(a => a.CandidateId == candidate.Id && a.JobPostingId == request.JobPostingId, ct);

        if (alreadyApplied)
            return Result<ApplyToJobDto>.Failure("You have already applied to this position.");

        // Run AI scoring + parsing in parallel
        int? aiScore = null;
        if (!string.IsNullOrWhiteSpace(request.ResumeText))
        {
            var candidateIdStr = candidate.Id.ToString();
            var scoreTask = _aiEngine.ScoreResumeAsync(candidateIdStr, request.ResumeText, ct);
            var parseTask = _aiEngine.ParseResumeAsync(candidateIdStr, request.ResumeText, ct);
            await Task.WhenAll(scoreTask, parseTask);

            var raw = scoreTask.Result;
            aiScore = raw.HasValue ? (int)Math.Round(raw.Value) : null;
            candidate.AiScore = aiScore;

            var parsed = parseTask.Result;
            if (parsed is not null)
            {
                candidate.ParsedSkillsJson = JsonSerializer.Serialize(parsed.Skills, _opts);
                candidate.ParsedExperienceJson = JsonSerializer.Serialize(parsed.Experience, _opts);
                candidate.ParsedEducationJson = JsonSerializer.Serialize(parsed.Education, _opts);
                candidate.ParsedSummary = parsed.Summary;
            }
        }

        // Create the application
        var application = new JobApplication
        {
            CandidateId = candidate.Id,
            JobPostingId = request.JobPostingId,
            MatchScore = aiScore,
            Notes = "Applied via job portal"
        };

        await _context.JobApplications.AddAsync(application, ct);
        await _context.SaveChangesAsync(ct);

        return Result<ApplyToJobDto>.Success(new ApplyToJobDto(
            application.Id,
            candidate.Id,
            $"{candidate.FirstName} {candidate.LastName}",
            candidate.Email,
            job.Id,
            job.Title,
            aiScore,
            "Your application has been submitted successfully. We will be in touch soon!"));
    }
}
