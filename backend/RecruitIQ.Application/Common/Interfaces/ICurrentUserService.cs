namespace RecruitIQ.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Role { get; }
    bool IsAdmin { get; }
}
