using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Auth.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Infrastructure.Persistence;

namespace RecruitIQ.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwt;

    public AuthController(AppDbContext db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { error = "Invalid email or password." });

        var token = _jwt.GenerateToken(user);

        return Ok(new AuthResponse(
            token,
            user.Email,
            $"{user.FirstName} {user.LastName}",
            user.Role,
            _jwt.GetExpiry()));
    }
}
