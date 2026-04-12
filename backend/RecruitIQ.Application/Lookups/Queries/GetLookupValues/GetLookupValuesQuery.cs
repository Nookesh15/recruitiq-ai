using MediatR;
using RecruitIQ.Application.Lookups.DTOs;

namespace RecruitIQ.Application.Lookups.Queries.GetLookupValues;

public record GetLookupValuesQuery(string Category) : IRequest<IReadOnlyList<LookupValueDto>>;
