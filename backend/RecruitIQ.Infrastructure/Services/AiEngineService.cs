using System.Net.Http.Json;
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
            // AI engine is optional — don't fail the upload if it's unavailable
            return null;
        }
    }

    private record AiScoreResult(double overall_score);
}
