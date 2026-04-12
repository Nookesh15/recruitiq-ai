using MediatR;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Commands.UploadResume;

public record UploadResumeCommand(Guid CandidateId, string ResumeUrl, string ResumeText) : IRequest<Result<CandidateDto>>;
