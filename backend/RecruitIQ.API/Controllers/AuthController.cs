using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Auth.DTOs;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Domain.Entities;
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

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListUsers(CancellationToken ct)
    {
        var users = await _db.Users
            .AsNoTracking()
            .Select(u => new { u.Id, u.Email, u.FirstName, u.LastName, u.Role })
            .ToListAsync(ct);
        return Ok(users);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email, ct))
            return Conflict(new { error = "Email already in use." });

        var role = request.Role is "Admin" or "Recruiter" ? request.Role : "Recruiter";

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return Created("", new { user.Id, user.Email, user.FirstName, user.LastName, user.Role });
    }
}

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Role = "Recruiter");
