// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace InterviewSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Role { get; set; }
}