namespace RecruitIQ.Application.Common.Interfaces;

public record ParsedExperience(string Role, string Company, string Duration);
public record ParsedEducation(string Degree, string? Field, string? Institution, string? Year);

public record ResumeParseResult(
    List<string> Skills,
    List<ParsedExperience> Experience,
    List<ParsedEducation> Education,
    string Summary
);

public interface IAiEngineService
{
    Task<double?> ScoreResumeAsync(string candidateId, string resumeText, CancellationToken ct = default);
    Task<ResumeParseResult?> ParseResumeAsync(string candidateId, string resumeText, CancellationToken ct = default);
}
