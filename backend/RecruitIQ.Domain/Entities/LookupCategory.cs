namespace RecruitIQ.Domain.Entities;

public class LookupCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<LookupValue> Values { get; set; } = [];
}
