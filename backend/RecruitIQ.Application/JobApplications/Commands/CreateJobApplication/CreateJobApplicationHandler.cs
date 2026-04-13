using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobApplications.DTOs;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Application.JobApplications.Commands.CreateJobApplication;

public class CreateJobApplicationHandler : IRequestHandler<CreateJobApplicationCommand, Result<JobApplicationDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateJobApplicationHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<JobApplicationDto>> Handle(CreateJobApplicationCommand request, CancellationToken ct)
    {
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == request.CandidateId, ct);
        if (candidate is null)
            return Result<JobApplicationDto>.Failure("Candidate not found.");

        var job = await _context.JobPostings
            .FirstOrDefaultAsync(j => j.Id == request.JobPostingId, ct);
        if (job is null)
            return Result<JobApplicationDto>.Failure("Job posting not found.");

        var exists = await _context.JobApplications
            .AnyAsync(a => a.CandidateId == request.CandidateId && a.JobPostingId == request.JobPostingId, ct);
        if (exists)
            return Result<JobApplicationDto>.Failure("Candidate has already applied to this job.");

        var application = new JobApplication
        {
            CandidateId = request.CandidateId,
            JobPostingId = request.JobPostingId,
            MatchScore = candidate.AiScore,
            Notes = request.Notes
        };

        await _context.JobApplications.AddAsync(application, ct);
        await _context.SaveChangesAsync(ct);

        return Result<JobApplicationDto>.Success(new JobApplicationDto(
            application.Id,
            candidate.Id,
            $"{candidate.FirstName} {candidate.LastName}",
            candidate.Email,
            job.Id,
            job.Title,
            application.MatchScore,
            application.Notes,
            application.CreatedAt,
            MatchReason: null,
            Strengths: null,
            Gaps: null));
    }
}
