using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Extensions.Logging;

namespace RecruitIQ.API.Services;

/// <summary>
/// Extracts plain text from uploaded resume files (PDF, DOCX, DOC).
/// All methods are best-effort — exceptions are caught and empty string returned.
/// </summary>
public static class ResumeTextExtractor
{
    private const int MaxChars = 8000;

    public static string Extract(string filePath, string extension, ILogger logger)
    {
        try
        {
            var text = extension switch
            {
                ".pdf"  => ExtractPdf(filePath),
                ".docx" => ExtractDocx(filePath),
                ".doc"  => string.Empty, // Binary DOC — not supported without COM interop
                _       => string.Empty,
            };

            return text.Length > MaxChars ? text[..MaxChars] : text;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Resume text extraction failed for {File} — AI scoring will be skipped", filePath);
            return string.Empty;
        }
    }

    private static string ExtractPdf(string filePath)
    {
        var sb = new System.Text.StringBuilder();

        using var reader = new PdfReader(filePath);
        for (var i = 1; i <= reader.NumberOfPages; i++)
        {
            var strategy = new LocationTextExtractionStrategy();
            var pageText = PdfTextExtractor.GetTextFromPage(reader, i, strategy);
            sb.AppendLine(pageText);

            // Stop early once we have enough text
            if (sb.Length >= MaxChars) break;
        }

        return sb.ToString().Trim();
    }

    private static string ExtractDocx(string filePath)
    {
        var sb = new System.Text.StringBuilder();

        using var doc = WordprocessingDocument.Open(filePath, isEditable: false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body is null) return string.Empty;

        foreach (var para in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
        {
            sb.AppendLine(para.InnerText);
            if (sb.Length >= MaxChars) break;
        }

        return sb.ToString().Trim();
    }
}
