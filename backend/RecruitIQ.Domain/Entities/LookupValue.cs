namespace RecruitIQ.Domain.Entities;

public class LookupValue : BaseEntity
{
    public Guid CategoryId { get; set; }
    public LookupCategory Category { get; set; } = null!;

    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
