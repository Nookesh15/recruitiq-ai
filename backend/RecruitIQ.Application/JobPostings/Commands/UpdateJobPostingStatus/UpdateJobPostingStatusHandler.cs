using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;

namespace RecruitIQ.Application.JobPostings.Commands.UpdateJobPostingStatus;

public class UpdateJobPostingStatusHandler : IRequestHandler<UpdateJobPostingStatusCommand, Result<JobPostingDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateJobPostingStatusHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<JobPostingDto>> Handle(UpdateJobPostingStatusCommand request, CancellationToken ct)
    {
        var job = await _context.JobPostings
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == request.Id, ct);

        if (job is null)
            return Result<JobPostingDto>.Failure("Job posting not found.");

        job.Status = request.Status;
        await _context.SaveChangesAsync(ct);

        return Result<JobPostingDto>.Success(new JobPostingDto(
            job.Id, job.Title, job.Description, job.Department,
            job.Location, job.Status, job.Applications.Count, job.CreatedAt));
    }
}
