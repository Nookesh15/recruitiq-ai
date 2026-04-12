using MediatR;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Models;
using RecruitIQ.Domain.Enums;

namespace RecruitIQ.Application.Candidates.Commands.UpdateCandidateStatus;

public record UpdateCandidateStatusCommand(Guid Id, CandidateStatus Status) : IRequest<Result<CandidateDto>>;
