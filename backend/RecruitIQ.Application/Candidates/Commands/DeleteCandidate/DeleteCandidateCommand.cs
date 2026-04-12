using MediatR;
using RecruitIQ.Application.Common.Models;

namespace RecruitIQ.Application.Candidates.Commands.DeleteCandidate;

public record DeleteCandidateCommand(Guid Id) : IRequest<Result>;
