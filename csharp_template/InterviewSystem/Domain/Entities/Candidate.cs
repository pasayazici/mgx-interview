// Domain/Entities/Candidate.cs
using InterviewSystem.Domain.Common;

namespace InterviewSystem.Domain.Entities;

public class Candidate : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public Guid InterviewId { get; set; }
    public InterviewStatus Status { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime? InterviewDate { get; set; }
    public string? VideoUrl { get; set; }
    public int? Score { get; set; }
    public string? Feedback { get; set; }
    public virtual Interview Interview { get; set; } = null!;
}

public enum InterviewStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Analyzed
}