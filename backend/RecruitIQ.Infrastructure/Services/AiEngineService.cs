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

    public async Task<ScoreResult?> ScoreResumeAsync(string candidateId, string resumeText, CancellationToken ct = default)
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
            if (result is null) return null;

            return new ScoreResult(
                Score: result.overall_score,
                MatchReason: result.match_reason ?? string.Empty,
                Strengths: result.strengths ?? [],
                Gaps: result.gaps ?? []
            );
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

    public async Task<JdAnalysisResult?> AnalyzeJdAsync(string jdText, CancellationToken ct = default)
    {
        try
        {
            var payload = new { jd_text = jdText };
            var response = await _http.PostAsJsonAsync("/api/v1/analyze-jd", payload, ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<AiJdAnalysis>(cancellationToken: ct);
            if (result is null) return null;

            return new JdAnalysisResult(
                FlagCount: result.flag_count,
                BiasFlags: result.bias_flags
                    .Select(f => new BiasFlag(f.phrase, f.category, f.suggestion))
                    .ToList(),
                IsClean: result.is_clean
            );
        }
        catch
        {
            return null;
        }
    }

    // ── Internal JSON mapping records ──────────────────────────────────────
    private record AiScoreResult(
        double overall_score,
        string? match_reason,
        List<string>? strengths,
        List<string>? gaps
    );

    private record AiParseResult(
        List<string> skills,
        List<AiExperience> experience,
        List<AiEducation> education,
        string summary
    );

    private record AiExperience(string role, string company, string duration);

    private record AiJdAnalysis(
        int flag_count,
        List<AiBiasFlag> bias_flags,
        bool is_clean
    );

    private record AiBiasFlag(string phrase, string category, string suggestion);

    private record AiEducation(
        string degree,
        [property: JsonPropertyName("field")] string? field,
        string? institution,
        string? year
    );
}
