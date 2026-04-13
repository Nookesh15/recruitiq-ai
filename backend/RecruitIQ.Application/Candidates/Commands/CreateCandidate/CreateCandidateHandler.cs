using MediatR;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Application.Candidates.Commands.CreateCandidate;

public class CreateCandidateHandler : IRequestHandler<CreateCandidateCommand, Result<CandidateDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCandidateHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CandidateDto>> Handle(CreateCandidateCommand request, CancellationToken ct)
    {
        var candidate = new Candidate
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            CreatedById = _currentUser.UserId
        };

        await _context.Candidates.AddAsync(candidate, ct);
        await _context.SaveChangesAsync(ct);

        return Result<CandidateDto>.Success(candidate.ToDto());
    }
}
