using RecruitIQ.Domain.Enums;

namespace RecruitIQ.Domain.Entities;

public class JobPosting : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Draft;

    public ICollection<JobApplication> Applications { get; set; } = [];
}
