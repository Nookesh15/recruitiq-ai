using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Domain.Interfaces;

public interface ICandidateRepository : IRepository<Candidate>
{
    Task<Candidate?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<Candidate>> GetByStatusAsync(string status, CancellationToken ct = default);
}
