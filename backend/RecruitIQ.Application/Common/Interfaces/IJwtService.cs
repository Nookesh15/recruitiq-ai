using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpiry();
}
