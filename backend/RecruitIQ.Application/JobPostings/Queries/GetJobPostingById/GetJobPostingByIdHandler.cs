using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;

namespace RecruitIQ.Application.JobPostings.Queries.GetJobPostingById;

public class GetJobPostingByIdHandler : IRequestHandler<GetJobPostingByIdQuery, Result<JobPostingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetJobPostingByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<JobPostingDto>> Handle(GetJobPostingByIdQuery request, CancellationToken ct)
    {
        var dto = await _context.JobPostings
            .AsNoTracking()
            .Where(j => j.Id == request.Id)
            .Select(j => new JobPostingDto(
                j.Id, j.Title, j.Description, j.Department,
                j.Location, j.Status,
                j.Applications.Count(a => !a.IsDeleted),
                j.CreatedAt))
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<JobPostingDto>.Failure("Job posting not found.");

        return Result<JobPostingDto>.Success(dto);
    }
}
