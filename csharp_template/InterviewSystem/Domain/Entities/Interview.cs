// Domain/Entities/Interview.cs
using InterviewSystem.Domain.Common;

namespace InterviewSystem.Domain.Entities;

public class Interview : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public int Duration { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid FirmId { get; set; }
    public virtual Company Company { get; set; } = null!;
    public virtual ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
}