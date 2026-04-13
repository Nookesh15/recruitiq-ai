using RecruitIQ.Domain.Enums;

namespace RecruitIQ.Application.Candidates.DTOs;

public record ParsedExperienceDto(string Role, string Company, string Duration);
public record ParsedEducationDto(string Degree, string? Field, string? Institution, string? Year);

public record CandidateDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? ResumeUrl,
    CandidateStatus Status,
    double? AiScore,
    DateTime CreatedAt,
    // Structured resume sections (null if not yet parsed)
    List<string>? Skills,
    List<ParsedExperienceDto>? Experience,
    List<ParsedEducationDto>? Education,
    string? ParsedSummary
);
