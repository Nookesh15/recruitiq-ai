using MediatR;
using Microsoft.EntityFrameworkCore;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Application.Lookups.DTOs;

namespace RecruitIQ.Application.Lookups.Queries.GetLookupValues;

public class GetLookupValuesHandler : IRequestHandler<GetLookupValuesQuery, IReadOnlyList<LookupValueDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLookupValuesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<LookupValueDto>> Handle(GetLookupValuesQuery request, CancellationToken ct)
    {
        return await _context.LookupValues
            .Where(v => v.Category.Name == request.Category && v.IsActive && !v.IsDeleted)
            .OrderBy(v => v.SortOrder)
            .Select(v => new LookupValueDto(v.Id, v.Code, v.DisplayName, v.SortOrder))
            .ToListAsync(ct);
    }
}
