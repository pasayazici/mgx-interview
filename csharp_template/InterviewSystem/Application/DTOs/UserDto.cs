// InterviewSystem/Application/DTOs/UserDto.cs
using System.ComponentModel.DataAnnotations;

namespace InterviewSystem.Application.DTOs;

public class UserDto
{
    public string? Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }

    [Required]
    public string Role { get; set; } = "User";

    public Guid? FirmId { get; set; }
    public string? CompanyName { get; set; }
    public bool IsDeleted { get; set; }
}