using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Commands.UploadResume;

public class UploadResumeHandler : IRequestHandler<UploadResumeCommand, Result<CandidateDto>>
{
    private static readonly JsonSerializerOptions _opts = new(JsonSerializerDefaults.Web);
    private readonly IApplicationDbContext _context;
    private readonly IAiEngineService _aiEngine;

    public UploadResumeHandler(IApplicationDbContext context, IAiEngineService aiEngine)
    {
        _context = context;
        _aiEngine = aiEngine;
    }

    public async Task<Result<CandidateDto>> Handle(UploadResumeCommand request, CancellationToken ct)
    {
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == request.CandidateId, ct);

        if (candidate is null)
            return Result<CandidateDto>.Failure("Candidate not found.");

        candidate.ResumeUrl = request.ResumeUrl;

        if (!string.IsNullOrWhiteSpace(request.ResumeText))
        {
            var candidateIdStr = request.CandidateId.ToString();

            // Run scoring and parsing in parallel
            var scoreTask = _aiEngine.ScoreResumeAsync(candidateIdStr, request.ResumeText, ct);
            var parseTask = _aiEngine.ParseResumeAsync(candidateIdStr, request.ResumeText, ct);

            await Task.WhenAll(scoreTask, parseTask);

            candidate.AiScore = scoreTask.Result?.Score;

            var parsed = parseTask.Result;
            if (parsed is not null)
            {
                candidate.ParsedSkillsJson = JsonSerializer.Serialize(parsed.Skills, _opts);
                candidate.ParsedExperienceJson = JsonSerializer.Serialize(parsed.Experience, _opts);
                candidate.ParsedEducationJson = JsonSerializer.Serialize(parsed.Education, _opts);
                candidate.ParsedSummary = parsed.Summary;
            }
        }

        await _context.SaveChangesAsync(ct);

        return Result<CandidateDto>.Success(candidate.ToDto());
    }
}
