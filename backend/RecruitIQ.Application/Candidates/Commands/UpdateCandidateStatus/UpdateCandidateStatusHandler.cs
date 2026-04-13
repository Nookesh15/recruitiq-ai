using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Commands.UpdateCandidateStatus;

public class UpdateCandidateStatusHandler : IRequestHandler<UpdateCandidateStatusCommand, Result<CandidateDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateCandidateStatusHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CandidateDto>> Handle(UpdateCandidateStatusCommand request, CancellationToken ct)
    {
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (candidate is null)
            return Result<CandidateDto>.Failure("Candidate not found.");

        candidate.Status = request.Status;
        await _context.SaveChangesAsync(ct);

        return Result<CandidateDto>.Success(candidate.ToDto());
    }
}
