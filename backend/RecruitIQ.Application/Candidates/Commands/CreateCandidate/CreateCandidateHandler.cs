using MediatR;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Application.Candidates.Commands.CreateCandidate;

public class CreateCandidateHandler : IRequestHandler<CreateCandidateCommand, Result<CandidateDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateCandidateHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CandidateDto>> Handle(CreateCandidateCommand request, CancellationToken ct)
    {
        var candidate = new Candidate
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone
        };

        await _context.Candidates.AddAsync(candidate, ct);
        await _context.SaveChangesAsync(ct);

        return Result<CandidateDto>.Success(new CandidateDto(
            candidate.Id,
            candidate.FirstName,
            candidate.LastName,
            candidate.Email,
            candidate.Phone,
            candidate.ResumeUrl,
            candidate.Status,
            candidate.AiScore,
            candidate.CreatedAt
        ));
    }
}
