using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Commands.DeleteCandidate;

public class DeleteCandidateHandler : IRequestHandler<DeleteCandidateCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteCandidateHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteCandidateCommand request, CancellationToken ct)
    {
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (candidate is null)
            return Result.Failure("Candidate not found.");

        _context.Candidates.Remove(candidate); // intercepted as soft-delete by SaveChangesAsync
        await _context.SaveChangesAsync(ct);

        return Result.Success();
    }
}
