using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecruitIQ.Application.Lookups.Queries.GetLookupValues;

namespace RecruitIQ.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LookupsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LookupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{category}")]
    public async Task<IActionResult> GetByCategory(string category, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetLookupValuesQuery(category), ct);
        return Ok(result);
    }
}
