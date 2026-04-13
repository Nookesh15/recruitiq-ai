using System.Net.Http.Json;
using System.Text.Json.Serialization;
using RecruitIQ.Application.Common.Interfaces;

namespace RecruitIQ.Infrastructure.Services;

public class AiEngineService : IAiEngineService
{
    private readonly HttpClient _http;

    public AiEngineService(HttpClient http)
    {
        _http = http;
    }

    public async Task<double?> ScoreResumeAsync(string candidateId, string resumeText, CancellationToken ct = default)
    {
        try
        {
            var payload = new
            {
                candidate_id = candidateId,
                resume_text = resumeText,
                job_description = "General candidate evaluation",
                required_skills = Array.Empty<string>()
            };

            var response = await _http.PostAsJsonAsync("/api/v1/score", payload, ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<AiScoreResult>(cancellationToken: ct);
            return result?.overall_score;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ResumeParseResult?> ParseResumeAsync(string candidateId, string resumeText, CancellationToken ct = default)
    {
        try
        {
            var payload = new { candidate_id = candidateId, resume_text = resumeText };
            var response = await _http.PostAsJsonAsync("/api/v1/parse", payload, ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<AiParseResult>(cancellationToken: ct);
            if (result is null) return null;

            return new ResumeParseResult(
                Skills: result.skills,
                Experience: result.experience
                    .Select(e => new ParsedExperience(e.role, e.company, e.duration))
                    .ToList(),
                Education: result.education
                    .Select(e => new ParsedEducation(e.degree, e.field, e.institution, e.year))
                    .ToList(),
                Summary: result.summary
            );
        }
        catch
        {
            return null;
        }
    }

    // ── Internal JSON mapping records ──────────────────────────────────────
    private record AiScoreResult(double overall_score);

    private record AiParseResult(
        List<string> skills,
        List<AiExperience> experience,
        List<AiEducation> education,
        string summary
    );

    private record AiExperience(string role, string company, string duration);

    private record AiEducation(
        string degree,
        [property: JsonPropertyName("field")] string? field,
        string? institution,
        string? year
    );
}
