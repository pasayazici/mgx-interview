// Models/Candidate.cs
namespace InterviewSystem.Models;

public class Candidate
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ResumeUrl { get; set; }
    public int InterviewId { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Interview Interview { get; set; }
}