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
        var job = await _context.JobPostings
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == request.Id, ct);

        if (job is null)
            return Result<JobPostingDto>.Failure("Job posting not found.");

        return Result<JobPostingDto>.Success(new JobPostingDto(
            job.Id, job.Title, job.Description, job.Department,
            job.Location, job.Status, job.Applications.Count, job.CreatedAt));
    }
}
