// InterviewSystem/Application/DTOs/CandidateDto.cs
using InterviewSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace InterviewSystem.Application.DTOs;

public class CandidateDto
{
    public string? Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public Guid InterviewId { get; set; }
    public string? InterviewTitle { get; set; }
    public InterviewStatus Status { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime? InterviewDate { get; set; }
    public string? VideoUrl { get; set; }
    public int? Score { get; set; }
    public string? Feedback { get; set; }
    public bool IsDeleted { get; set; }
}