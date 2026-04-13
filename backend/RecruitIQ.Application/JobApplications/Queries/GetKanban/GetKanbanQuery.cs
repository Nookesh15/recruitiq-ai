using MediatR;
using RecruitIQ.Application.JobApplications.DTOs;

namespace RecruitIQ.Application.JobApplications.Queries.GetKanban;

/// <summary>Returns all applications ordered by stage + score — used for the Kanban board.</summary>
/// <param name="JobPostingId">Optional filter; null returns all jobs.</param>
public record GetKanbanQuery(Guid? JobPostingId) : IRequest<IReadOnlyList<JobApplicationDto>>;
