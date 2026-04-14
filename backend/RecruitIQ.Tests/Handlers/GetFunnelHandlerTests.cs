using RecruitIQ.Application.Analytics.Queries;
using RecruitIQ.Domain.Entities;
using RecruitIQ.Domain.Enums;
using RecruitIQ.Infrastructure.Persistence;
using RecruitIQ.Tests.Helpers;

namespace RecruitIQ.Tests.Handlers;

public class GetFunnelHandlerTests
{
    private static async Task<(Guid candidateId, Guid jobId)> SeedBaseAsync(AppDbContext db)
    {
        var candidate = new Candidate { FirstName = "A", LastName = "B", Email = "a@b.com" };
        var job = new JobPosting
        {
            Title = "Dev", Description = "Dev job", Department = "Eng", Location = "Remote",
        };
        db.Candidates.Add(candidate);
        db.JobPostings.Add(job);
        await db.SaveChangesAsync();
        return (candidate.Id, job.Id);
    }

    [Fact]
    public async Task Handle_NoApplications_ReturnsZeroTotal()
    {
        await using var db = TestDbContext.Create();
        var handler = new GetFunnelHandler(db);

        var result = await handler.Handle(new GetFunnelQuery(null), CancellationToken.None);

        Assert.Equal(0, result.Total);
        Assert.Equal(6, result.Funnel.Count); // All 6 stages present
        Assert.All(result.Funnel, item => Assert.Equal(0, item.Count));
    }

    [Fact]
    public async Task Handle_ApplicationsInStages_CountsCorrectly()
    {
        await using var db = TestDbContext.Create();
        var (candidateId, jobId) = await SeedBaseAsync(db);

        db.JobApplications.AddRange(
            new JobApplication { CandidateId = candidateId, JobPostingId = jobId, Stage = "Applied" },
            new JobApplication { CandidateId = candidateId, JobPostingId = jobId, Stage = "Applied" },
            new JobApplication { CandidateId = candidateId, JobPostingId = jobId, Stage = "Interview" }
        );
        await db.SaveChangesAsync();

        var handler = new GetFunnelHandler(db);
        var result = await handler.Handle(new GetFunnelQuery(null), CancellationToken.None);

        Assert.Equal(3, result.Total);
        Assert.Equal(2, result.Funnel.First(f => f.Stage == "Applied").Count);
        Assert.Equal(1, result.Funnel.First(f => f.Stage == "Interview").Count);
        Assert.Equal(0, result.Funnel.First(f => f.Stage == "Hired").Count);
    }

    [Fact]
    public async Task Handle_WithJobFilter_OnlyCountsMatchingJob()
    {
        await using var db = TestDbContext.Create();
        var (candidateId, jobId) = await SeedBaseAsync(db);

        var otherJob = new JobPosting
        {
            Title = "Other", Description = "Other job", Department = "HR", Location = "NYC",
        };
        db.JobPostings.Add(otherJob);
        await db.SaveChangesAsync();

        db.JobApplications.AddRange(
            new JobApplication { CandidateId = candidateId, JobPostingId = jobId, Stage = "Applied" },
            new JobApplication { CandidateId = candidateId, JobPostingId = otherJob.Id, Stage = "Hired" }
        );
        await db.SaveChangesAsync();

        var handler = new GetFunnelHandler(db);
        var result = await handler.Handle(new GetFunnelQuery(jobId), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Equal(1, result.Funnel.First(f => f.Stage == "Applied").Count);
        Assert.Equal(0, result.Funnel.First(f => f.Stage == "Hired").Count);
    }

    [Fact]
    public async Task Handle_WithMatchScores_ComputesAvgScore()
    {
        await using var db = TestDbContext.Create();
        var (candidateId, jobId) = await SeedBaseAsync(db);

        db.JobApplications.AddRange(
            new JobApplication { CandidateId = candidateId, JobPostingId = jobId, Stage = "Applied", MatchScore = 80 },
            new JobApplication { CandidateId = candidateId, JobPostingId = jobId, Stage = "Applied", MatchScore = 60 }
        );
        await db.SaveChangesAsync();

        var handler = new GetFunnelHandler(db);
        var result = await handler.Handle(new GetFunnelQuery(null), CancellationToken.None);

        Assert.NotNull(result.AvgScore);
        Assert.Equal(70.0, result.AvgScore);
    }
}
