using System.Text.Json;
using RecruitIQ.Domain.Entities;

namespace RecruitIQ.Application.JobApplications.DTOs;

internal static class JobApplicationMappingExtensions
{
    private static readonly JsonSerializerOptions _opts = new(JsonSerializerDefaults.Web);

    public static JobApplicationDto ToDto(this JobApplication a) => new(
        a.Id,
        a.Candidate.Id,
        $"{a.Candidate.FirstName} {a.Candidate.LastName}",
        a.Candidate.Email,
        a.JobPosting.Id,
        a.JobPosting.Title,
        a.MatchScore,
        a.Notes,
        a.CreatedAt,
        MatchReason: a.MatchReason,
        Strengths: Deserialize<List<string>>(a.StrengthsJson),
        Gaps: Deserialize<List<string>>(a.GapsJson)
    );

    private static T? Deserialize<T>(string? json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try { return JsonSerializer.Deserialize<T>(json, _opts); }
        catch { return null; }
    }
}
