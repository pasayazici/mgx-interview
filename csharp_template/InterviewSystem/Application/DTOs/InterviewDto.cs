// InterviewSystem/Application/DTOs/InterviewDto.cs
using System.ComponentModel.DataAnnotations;

namespace InterviewSystem.Application.DTOs;

public class InterviewDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Question { get; set; } = string.Empty;

    [Required]
    public string Details { get; set; } = string.Empty;

    [Required]
    [Range(1, 180)]
    public int Duration { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public Guid FirmId { get; set; }
    public string? CompanyName { get; set; }
    public bool IsDeleted { get; set; }
}