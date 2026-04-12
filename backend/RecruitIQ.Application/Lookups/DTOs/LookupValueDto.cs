namespace RecruitIQ.Application.Lookups.DTOs;

public record LookupValueDto(
    Guid Id,
    string Code,
    string DisplayName,
    int SortOrder
);
