namespace RecruitIQ.Application.Common.Interfaces;

public interface IAiEngineService
{
    Task<double?> ScoreResumeAsync(string candidateId, string resumeText, CancellationToken ct = default);
}
