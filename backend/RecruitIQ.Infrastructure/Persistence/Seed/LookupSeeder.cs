using Microsoft.EntityFrameworkCore;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Infrastructure.Persistence.Seed;

public static class LookupSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.LookupCategories.AnyAsync()) return;

        var categories = new List<(string Name, string Description, List<(string Code, string Display, int Order)> Values)>
        {
            (
                "CandidateStatus", "Stages in the candidate hiring journey",
                new()
                {
                    ("Applied",   "Applied",    1),
                    ("Screening", "Screening",  2),
                    ("Interview", "Interview",  3),
                    ("Offer",     "Offer Sent", 4),
                    ("Hired",     "Hired",      5),
                    ("Rejected",  "Rejected",   6),
                }
            ),
            (
                "JobStatus", "Status of a job posting",
                new()
                {
                    ("Draft",   "Draft",  1),
                    ("Active",  "Active", 2),
                    ("Paused",  "Paused", 3),
                    ("Closed",  "Closed", 4),
                }
            ),
            (
                "Department", "Company departments",
                new()
                {
                    ("Engineering",     "Engineering",      1),
                    ("Product",         "Product",          2),
                    ("Design",          "Design",           3),
                    ("HR",              "Human Resources",  4),
                    ("Finance",         "Finance",          5),
                    ("Sales",           "Sales",            6),
                    ("Marketing",       "Marketing",        7),
                    ("Operations",      "Operations",       8),
                }
            ),
            (
                "EmploymentType", "Type of employment",
                new()
                {
                    ("FullTime",   "Full Time",   1),
                    ("PartTime",   "Part Time",   2),
                    ("Contract",   "Contract",    3),
                    ("Internship", "Internship",  4),
                    ("Remote",     "Remote",      5),
                }
            )
        };

        foreach (var (name, description, values) in categories)
        {
            var category = new LookupCategory
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description
            };

            await context.LookupCategories.AddAsync(category);

            foreach (var (code, display, order) in values)
            {
                await context.LookupValues.AddAsync(new LookupValue
                {
                    Id = Guid.NewGuid(),
                    CategoryId = category.Id,
                    Code = code,
                    DisplayName = display,
                    SortOrder = order,
                    IsActive = true
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
