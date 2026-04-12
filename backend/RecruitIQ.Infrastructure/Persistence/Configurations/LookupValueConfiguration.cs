using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Infrastructure.Persistence.Configurations;

public class LookupValueConfiguration : IEntityTypeConfiguration<LookupValue>
{
    public void Configure(EntityTypeBuilder<LookupValue> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Code).IsRequired().HasMaxLength(50);
        builder.Property(v => v.DisplayName).IsRequired().HasMaxLength(100);

        builder.HasOne(v => v.Category)
               .WithMany(c => c.Values)
               .HasForeignKey(v => v.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => new { v.CategoryId, v.Code }).IsUnique();
    }
}
