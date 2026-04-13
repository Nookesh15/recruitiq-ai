namespace RecruitIQ.Application.Common.Interfaces;

public interface IEmailService
{
    /// <summary>Sends an application confirmation to the candidate. Fire-and-forget.</summary>
    void SendApplicationConfirmation(string toEmail, string candidateName, string jobTitle);

    /// <summary>Sends a stage-change notification to the candidate. Fire-and-forget.</summary>
    void SendStageChange(string toEmail, string candidateName, string jobTitle, string newStage);
}
