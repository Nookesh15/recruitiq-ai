using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitIQ.Application.Common.Interfaces;

namespace RecruitIQ.API.Controllers;

[ApiController]
[Route("api/v1/ai")]
[Authorize]
public class AiController(IAiEngineService aiEngine) : ControllerBase
{
    /// <summary>Analyse a job description for biased or exclusionary language.</summary>
    [HttpPost("analyze-jd")]
    public async Task<IActionResult> AnalyzeJd(
        [FromBody] AnalyzeJdRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.JdText))
            return BadRequest(new { error = "jdText is required." });

        var result = await aiEngine.AnalyzeJdAsync(request.JdText, ct);

        if (result is null)
            return Ok(new { flagCount = 0, biasFlags = Array.Empty<object>(), isClean = true });

        return Ok(new
        {
            flagCount = result.FlagCount,
            biasFlags = result.BiasFlags.Select(f => new
            {
                phrase = f.Phrase,
                category = f.Category,
                suggestion = f.Suggestion,
            }),
            isClean = result.IsClean,
        });
    }

    public record AnalyzeJdRequest(string JdText);
}
