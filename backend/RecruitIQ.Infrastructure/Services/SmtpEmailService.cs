using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecruitIQ.Application.Common.Interfaces;

namespace RecruitIQ.Infrastructure.Services;

/// <summary>
/// Fire-and-forget email service using SMTP.
/// Configure via appsettings / environment:
///   Email:Host, Email:Port, Email:From, Email:Username, Email:Password, Email:EnableSsl
/// When Email:Host is not set, emails are silently skipped (dev/test mode).
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public void SendApplicationConfirmation(string toEmail, string candidateName, string jobTitle)
    {
        var subject = $"Application received — {jobTitle}";
        var body = BuildApplicationConfirmationHtml(candidateName, jobTitle);
        FireAndForget(toEmail, subject, body);
    }

    public void SendStageChange(string toEmail, string candidateName, string jobTitle, string newStage)
    {
        var subject = $"Application update — {jobTitle}";
        var body = BuildStageChangeHtml(candidateName, jobTitle, newStage);
        FireAndForget(toEmail, subject, body);
    }

    private void FireAndForget(string toEmail, string subject, string htmlBody)
    {
        var host = _config["Email:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogDebug("Email:Host not configured — skipping email to {To}: {Subject}", toEmail, subject);
            return;
        }

        // Capture config values before entering the task lambda
        var from    = _config["Email:From"] ?? "noreply@recruitiq.ai";
        var user    = _config["Email:Username"] ?? string.Empty;
        var pass    = _config["Email:Password"] ?? string.Empty;
        var port    = int.TryParse(_config["Email:Port"], out var p) ? p : 587;
        var ssl     = !string.Equals(_config["Email:EnableSsl"], "false", StringComparison.OrdinalIgnoreCase);

        _ = Task.Run(async () =>
        {
            try
            {
                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = ssl,
                    Credentials = string.IsNullOrWhiteSpace(user)
                        ? null
                        : new NetworkCredential(user, pass),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                using var message = new MailMessage(from, toEmail, subject, htmlBody)
                {
                    IsBodyHtml = true,
                };

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {To}: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send email to {To}: {Subject}", toEmail, subject);
            }
        });
    }

    private static string BuildApplicationConfirmationHtml(string name, string jobTitle) => $"""
        <!DOCTYPE html>
        <html>
        <body style="font-family:system-ui,sans-serif;background:#f9fafb;padding:32px;color:#1f2937">
          <div style="max-width:560px;margin:0 auto;background:#fff;border-radius:12px;padding:32px;border:1px solid #e5e7eb">
            <h1 style="font-size:1.25rem;font-weight:700;color:#4f46e5;margin-top:0">
              Recruit<span style="color:#111827">IQ</span>
              <span style="font-size:0.7rem;background:#ede9fe;color:#6d28d9;padding:2px 8px;border-radius:99px;vertical-align:middle;margin-left:6px">AI</span>
            </h1>
            <h2 style="font-size:1rem;font-weight:600;margin-bottom:8px">Application Received</h2>
            <p style="color:#6b7280;font-size:0.9rem">Hi {name},</p>
            <p style="color:#6b7280;font-size:0.9rem">
              Thank you for applying to <strong style="color:#111827">{jobTitle}</strong>.
              We have received your application and our team will review it shortly.
            </p>
            <p style="color:#6b7280;font-size:0.9rem">We will keep you updated on the status of your application.</p>
            <hr style="border:none;border-top:1px solid #e5e7eb;margin:24px 0"/>
            <p style="color:#9ca3af;font-size:0.75rem">RecruitIQ — AI-Powered Hiring Platform</p>
          </div>
        </body>
        </html>
        """;

    private static string BuildStageChangeHtml(string name, string jobTitle, string stage)
    {
        var stageMessage = stage switch
        {
            "Screening"  => "Your application is now under review with our screening team.",
            "Interview"  => "Congratulations! We would like to invite you for an interview. Our team will be in touch with next steps.",
            "Offer"      => "Great news! We are pleased to extend an offer. A team member will contact you with details.",
            "Hired"      => "Welcome aboard! We are excited to have you join the team.",
            "Rejected"   => "After careful consideration, we have decided to move forward with other candidates at this time. We appreciate your interest.",
            _            => $"Your application status has been updated to <strong>{stage}</strong>.",
        };

        return $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family:system-ui,sans-serif;background:#f9fafb;padding:32px;color:#1f2937">
              <div style="max-width:560px;margin:0 auto;background:#fff;border-radius:12px;padding:32px;border:1px solid #e5e7eb">
                <h1 style="font-size:1.25rem;font-weight:700;color:#4f46e5;margin-top:0">
                  Recruit<span style="color:#111827">IQ</span>
                  <span style="font-size:0.7rem;background:#ede9fe;color:#6d28d9;padding:2px 8px;border-radius:99px;vertical-align:middle;margin-left:6px">AI</span>
                </h1>
                <h2 style="font-size:1rem;font-weight:600;margin-bottom:8px">Application Update — {jobTitle}</h2>
                <p style="color:#6b7280;font-size:0.9rem">Hi {name},</p>
                <p style="color:#6b7280;font-size:0.9rem">{stageMessage}</p>
                <div style="background:#f3f4f6;border-radius:8px;padding:12px 16px;margin:16px 0;font-size:0.85rem">
                  <span style="color:#6b7280">Current status: </span>
                  <strong style="color:#4f46e5">{stage}</strong>
                </div>
                <hr style="border:none;border-top:1px solid #e5e7eb;margin:24px 0"/>
                <p style="color:#9ca3af;font-size:0.75rem">RecruitIQ — AI-Powered Hiring Platform</p>
              </div>
            </body>
            </html>
            """;
    }
}
