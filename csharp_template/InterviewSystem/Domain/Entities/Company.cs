// Domain/Entities/Company.cs
using InterviewSystem.Domain.Common;

namespace InterviewSystem.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}