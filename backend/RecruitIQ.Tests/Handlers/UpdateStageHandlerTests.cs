using NSubstitute;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.JobApplications.Commands.UpdateStage;
using RecruitIQ.Domain.Entities;
using RecruitIQ.Domain.Enums;
using RecruitIQ.Infrastructure.Persistence;
using RecruitIQ.Tests.Helpers;

namespace RecruitIQ.Tests.Handlers;

public class UpdateStageHandlerTests
{
    private static (UpdateStageHandler handler, AppDbContext db) Setup()
    {
        var db = TestDbContext.Create();
        var email = Substitute.For<IEmailService>();
        var handler = new UpdateStageHandler(db, email);
        return (handler, db);
    }

    private static async Task<JobApplication> SeedApplicationAsync(AppDbContext db)
    {
        var candidate = new Candidate
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
        };
        var job = new JobPosting
        {
            Title = "Software Engineer",
            Description = "A job",
            Department = "Engineering",
            Location = "Remote",
        };
        db.Candidates.Add(candidate);
        db.JobPostings.Add(job);
        await db.SaveChangesAsync();

        var app = new JobApplication
        {
            CandidateId = candidate.Id,
            JobPostingId = job.Id,
            Stage = "Applied",
        };
        db.JobApplications.Add(app);
        await db.SaveChangesAsync();
        return app;
    }

    [Fact]
    public async Task Handle_ValidStage_UpdatesApplicationStage()
    {
        var (handler, db) = Setup();
        await using (db)
        {
            var app = await SeedApplicationAsync(db);

            var result = await handler.Handle(
                new UpdateStageCommand(app.Id, "Interview"),
                CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Interview", result.Value!.Stage);
        }
    }

    [Fact]
    public async Task Handle_InvalidStage_ReturnsFailure()
    {
        var (handler, db) = Setup();
        await using (db)
        {
            var app = await SeedApplicationAsync(db);

            var result = await handler.Handle(
                new UpdateStageCommand(app.Id, "Bogus"),
                CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid stage", result.Error);
        }
    }

    [Fact]
    public async Task Handle_UnknownApplicationId_ReturnsFailure()
    {
        var (handler, db) = Setup();
        await using (db)
        {
            var result = await handler.Handle(
                new UpdateStageCommand(Guid.NewGuid(), "Screening"),
                CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Contains("not found", result.Error);
        }
    }

    [Fact]
    public async Task Handle_StageChange_FiresEmailNotification()
    {
        var db = TestDbContext.Create();
        var emailService = Substitute.For<IEmailService>();
        var handler = new UpdateStageHandler(db, emailService);

        await using (db)
        {
            var app = await SeedApplicationAsync(db);

            await handler.Handle(new UpdateStageCommand(app.Id, "Interview"), CancellationToken.None);

            emailService.Received(1).SendStageChange(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), "Interview");
        }
    }
}
