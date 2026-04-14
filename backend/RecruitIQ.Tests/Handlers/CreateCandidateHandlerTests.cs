using NSubstitute;
using RecruitIQ.Application.Candidates.Commands.CreateCandidate;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Tests.Helpers;

namespace RecruitIQ.Tests.Handlers;

public class CreateCandidateHandlerTests
{
    private static ICurrentUserService AdminUser()
    {
        var svc = Substitute.For<ICurrentUserService>();
        svc.UserId.Returns(Guid.NewGuid());
        svc.Role.Returns("Admin");
        svc.IsAdmin.Returns(true);
        return svc;
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCandidateDto()
    {
        await using var db = TestDbContext.Create();
        var handler = new CreateCandidateHandler(db, AdminUser());

        var command = new CreateCandidateCommand("Jane", "Doe", "jane@example.com", "555-1234");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Jane", result.Value!.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal("jane@example.com", result.Value.Email);
        Assert.Equal("555-1234", result.Value.Phone);
    }

    [Fact]
    public async Task Handle_PersistsCandidateToDatabase()
    {
        await using var db = TestDbContext.Create();
        var handler = new CreateCandidateHandler(db, AdminUser());

        var command = new CreateCandidateCommand("John", "Smith", "john@example.com", null);

        await handler.Handle(command, CancellationToken.None);

        var saved = db.Candidates.Single();
        Assert.Equal("John", saved.FirstName);
        Assert.Equal("john@example.com", saved.Email);
    }

    [Fact]
    public async Task Handle_SetsCreatedByIdFromCurrentUser()
    {
        await using var db = TestDbContext.Create();
        var userId = Guid.NewGuid();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(userId);
        currentUser.IsAdmin.Returns(false);

        var handler = new CreateCandidateHandler(db, currentUser);
        var command = new CreateCandidateCommand("Alice", "Jones", "alice@example.com", null);

        await handler.Handle(command, CancellationToken.None);

        var saved = db.Candidates.Single();
        Assert.Equal(userId, saved.CreatedById);
    }
}
