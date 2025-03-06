// Domain/Entities/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace InterviewSystem.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? FirmId { get; set; }
    public virtual Company? Company { get; set; }
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}