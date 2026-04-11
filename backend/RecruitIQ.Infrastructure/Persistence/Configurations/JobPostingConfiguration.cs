using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Infrastructure.Persistence.Configurations;

public class JobPostingConfiguration : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Title).IsRequired().HasMaxLength(200);
        builder.Property(j => j.Department).IsRequired().HasMaxLength(100);
        builder.Property(j => j.Location).IsRequired().HasMaxLength(100);
        builder.Property(j => j.Status).HasConversion<string>();

        builder.HasIndex(j => j.Status);
    }
}
