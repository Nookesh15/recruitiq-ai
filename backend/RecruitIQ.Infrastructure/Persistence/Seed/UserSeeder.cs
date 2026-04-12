using Microsoft.EntityFrameworkCore;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Infrastructure.Persistence.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        // Default admin — change password after first login in production
        db.Users.Add(new User
        {
            Email = "admin@recruitiq.ai",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            FirstName = "Admin",
            LastName = "User",
            Role = "Admin"
        });

        await db.SaveChangesAsync();
    }
}
