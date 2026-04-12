using Microsoft.EntityFrameworkCore;
using RecruitIQ.Domain.Entities;
using RecruitIQ.Domain.Interfaces;
using RecruitIQ.Infrastructure.Persistence;

namespace RecruitIQ.Infrastructure.Repositories;

public class LookupRepository : ILookupRepository
{
    private readonly AppDbContext _context;

    public LookupRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LookupCategory?> GetCategoryByNameAsync(string categoryName, CancellationToken ct = default)
        => await _context.LookupCategories
            .FirstOrDefaultAsync(c => c.Name == categoryName, ct);

    public async Task<IReadOnlyList<LookupValue>> GetValuesByCategoryAsync(string categoryName, CancellationToken ct = default)
        => await _context.LookupValues
            .Where(v => v.Category.Name == categoryName && v.IsActive)
            .OrderBy(v => v.SortOrder)
            .ToListAsync(ct);
}
