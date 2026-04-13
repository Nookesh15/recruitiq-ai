namespace RecruitIQ.Application.Common.Interfaces;

public record ParsedExperience(string Role, string Company, string Duration);
public record ParsedEducation(string Degree, string? Field, string? Institution, string? Year);

public record ResumeParseResult(
    List<string> Skills,
    List<ParsedExperience> Experience,
    List<ParsedEducation> Education,
    string Summary
);

public record ScoreResult(
    double Score,
    string MatchReason,
    List<string> Strengths,
    List<string> Gaps
);

public record BiasFlag(string Phrase, string Category, string Suggestion);

public record JdAnalysisResult(int FlagCount, List<BiasFlag> BiasFlags, bool IsClean);

public interface IAiEngineService
{
    Task<ScoreResult?> ScoreResumeAsync(string candidateId, string resumeText, CancellationToken ct = default);
    Task<ResumeParseResult?> ParseResumeAsync(string candidateId, string resumeText, CancellationToken ct = default);
    Task<JdAnalysisResult?> AnalyzeJdAsync(string jdText, CancellationToken ct = default);
}
