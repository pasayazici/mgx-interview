// Models/Interview.cs
namespace InterviewSystem.Models;

public class Interview
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Position { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Company Company { get; set; }
    public virtual ICollection<Candidate> Candidates { get; set; }
}