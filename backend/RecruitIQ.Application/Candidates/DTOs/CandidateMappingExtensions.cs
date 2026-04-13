using System.Text.Json;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Application.Candidates.DTOs;

internal static class CandidateMappingExtensions
{
    private static readonly JsonSerializerOptions _opts = new(JsonSerializerDefaults.Web);

    public static CandidateDto ToDto(this Candidate c) => new(
        c.Id, c.FirstName, c.LastName, c.Email,
        c.Phone, c.ResumeUrl, c.Status, c.AiScore, c.CreatedAt,
        Skills: Deserialize<List<string>>(c.ParsedSkillsJson),
        Experience: Deserialize<List<ParsedExperienceDto>>(c.ParsedExperienceJson),
        Education: Deserialize<List<ParsedEducationDto>>(c.ParsedEducationJson),
        ParsedSummary: c.ParsedSummary
    );

    private static T? Deserialize<T>(string? json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try { return JsonSerializer.Deserialize<T>(json, _opts); }
        catch { return null; }
    }
}
