// Models/Company.cs
namespace InterviewSystem.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Industry { get; set; }
    public string Location { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Interview> Interviews { get; set; }
}