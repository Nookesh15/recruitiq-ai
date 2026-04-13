using MediatR;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Commands.UpdateStage;

public record UpdateStageCommand(Guid ApplicationId, string Stage) : IRequest<Result<JobApplicationDto>>;
