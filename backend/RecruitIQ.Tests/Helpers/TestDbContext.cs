using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Infrastructure.Persistence;

namespace RecruitIQ.Tests.Helpers;

public static class TestDbContext
{
    /// <summary>Creates a fresh in-memory AppDbContext with a unique database name per call.</summary>
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
