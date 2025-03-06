// InterviewSystem/Application/DTOs/CompanyDto.cs
using System.ComponentModel.DataAnnotations;

namespace InterviewSystem.Application.DTOs;

public class CompanyDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}