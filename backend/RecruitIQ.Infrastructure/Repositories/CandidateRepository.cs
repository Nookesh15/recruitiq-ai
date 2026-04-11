using Microsoft.EntityFrameworkCore;
using RecruitIQ.Domain.Entities;
using RecruitIQ.Domain.Interfaces;
using RecruitIQ.Infrastructure.Persistence;

namespace RecruitIQ.Infrastructure.Repositories;

public class CandidateRepository : BaseRepository<Candidate>, ICandidateRepository
{
    public CandidateRepository(AppDbContext context) : base(context) { }

    public async Task<Candidate?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Email == email, ct);

    public async Task<IReadOnlyList<Candidate>> GetByStatusAsync(string status, CancellationToken ct = default)
        => await _dbSet.Where(c => c.Status.ToString() == status).ToListAsync(ct);
}
