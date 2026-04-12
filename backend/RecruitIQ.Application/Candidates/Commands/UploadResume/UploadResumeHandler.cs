using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Commands.UploadResume;

public class UploadResumeHandler : IRequestHandler<UploadResumeCommand, Result<CandidateDto>>
{
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

        // Fire AI scoring if resume text was extracted
        if (!string.IsNullOrWhiteSpace(request.ResumeText))
        {
            var score = await _aiEngine.ScoreResumeAsync(
                request.CandidateId.ToString(), request.ResumeText, ct);
            candidate.AiScore = score;
        }

        await _context.SaveChangesAsync(ct);

        return Result<CandidateDto>.Success(new CandidateDto(
            candidate.Id, candidate.FirstName, candidate.LastName, candidate.Email,
            candidate.Phone, candidate.ResumeUrl, candidate.Status, candidate.AiScore, candidate.CreatedAt));
    }
}
