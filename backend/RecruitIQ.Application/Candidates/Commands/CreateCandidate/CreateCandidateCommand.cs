using MediatR;
using RecruitIQ.Application.Candidates.DTOs;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Commands.CreateCandidate;

public record CreateCandidateCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone
) : IRequest<Result<CandidateDto>>;
