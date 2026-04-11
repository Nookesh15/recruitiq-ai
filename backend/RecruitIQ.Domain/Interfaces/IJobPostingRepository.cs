using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Domain.Interfaces;

public interface IJobPostingRepository : IRepository<JobPosting>
{
    Task<IReadOnlyList<JobPosting>> GetActiveJobsAsync(CancellationToken ct = default);
}
