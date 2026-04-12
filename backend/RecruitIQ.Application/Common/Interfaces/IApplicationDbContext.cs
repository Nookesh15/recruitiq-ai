using Microsoft.EntityFrameworkCore;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Candidate> Candidates { get; }
    DbSet<JobPosting> JobPostings { get; }
    DbSet<JobApplication> JobApplications { get; }
    DbSet<LookupCategory> LookupCategories { get; }
    DbSet<LookupValue> LookupValues { get; }
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
