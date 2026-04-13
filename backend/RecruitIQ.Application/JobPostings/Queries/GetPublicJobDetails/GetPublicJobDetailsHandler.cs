using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;

namespace RecruitIQ.Application.JobPostings.Queries.GetPublicJobDetails;

public class GetPublicJobDetailsHandler : IRequestHandler<GetPublicJobDetailsQuery, Result<PublicJobDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPublicJobDetailsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PublicJobDto>> Handle(GetPublicJobDetailsQuery request, CancellationToken ct)
    {
        var job = await _context.JobPostings
            .AsNoTracking()
            .Where(j => j.Id == request.JobId)
            .Select(j => new PublicJobDto(
                j.Id, j.Title, j.Description, j.Department,
                j.Location, j.Status.ToString(), j.CreatedAt))
            .FirstOrDefaultAsync(ct);

        if (job is null)
            return Result<PublicJobDto>.Failure("Job not found.");

        return Result<PublicJobDto>.Success(job);
    }
}
