using MediatR;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobPostings.DTOs;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Application.JobPostings.Commands.CreateJobPosting;

public class CreateJobPostingHandler : IRequestHandler<CreateJobPostingCommand, Result<JobPostingDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateJobPostingHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<JobPostingDto>> Handle(CreateJobPostingCommand request, CancellationToken ct)
    {
        var job = new JobPosting
        {
            Title = request.Title,
            Description = request.Description,
            Department = request.Department,
            Location = request.Location
        };

        await _context.JobPostings.AddAsync(job, ct);
        await _context.SaveChangesAsync(ct);

        return Result<JobPostingDto>.Success(new JobPostingDto(
            job.Id, job.Title, job.Description, job.Department,
            job.Location, job.Status, 0, job.CreatedAt));
    }
}
