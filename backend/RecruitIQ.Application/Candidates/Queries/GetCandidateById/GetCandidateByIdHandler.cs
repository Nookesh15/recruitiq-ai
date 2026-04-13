using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Queries.GetCandidateById;

public class GetCandidateByIdHandler : IRequestHandler<GetCandidateByIdQuery, Result<CandidateDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCandidateByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CandidateDto>> Handle(GetCandidateByIdQuery request, CancellationToken ct)
    {
        var c = await _context.Candidates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (c is null)
            return Result<CandidateDto>.Failure("Candidate not found.");

        return Result<CandidateDto>.Success(new CandidateDto(
            c.Id, c.FirstName, c.LastName, c.Email,
            c.Phone, c.ResumeUrl, c.Status, c.AiScore, c.CreatedAt));
    }
}
