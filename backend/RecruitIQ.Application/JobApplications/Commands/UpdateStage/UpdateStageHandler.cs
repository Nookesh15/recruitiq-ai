using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Commands.UpdateStage;

public class UpdateStageHandler : IRequestHandler<UpdateStageCommand, Result<JobApplicationDto>>
{
    private static readonly HashSet<string> ValidStages =
        ["Applied", "Screening", "Interview", "Offer", "Hired", "Rejected"];

    private readonly IApplicationDbContext _context;

    public UpdateStageHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<JobApplicationDto>> Handle(UpdateStageCommand request, CancellationToken ct)
    {
        if (!ValidStages.Contains(request.Stage))
            return Result<JobApplicationDto>.Failure($"Invalid stage '{request.Stage}'.");

        var application = await _context.JobApplications
            .Include(a => a.Candidate)
            .Include(a => a.JobPosting)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, ct);

        if (application is null)
            return Result<JobApplicationDto>.Failure("Application not found.");

        application.Stage = request.Stage;
        await _context.SaveChangesAsync(ct);

        return Result<JobApplicationDto>.Success(application.ToDto());
    }
}
