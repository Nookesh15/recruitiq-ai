using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Domain.Interfaces;

public interface ILookupRepository
{
    Task<LookupCategory?> GetCategoryByNameAsync(string categoryName, CancellationToken ct = default);
    Task<IReadOnlyList<LookupValue>> GetValuesByCategoryAsync(string categoryName, CancellationToken ct = default);
}
