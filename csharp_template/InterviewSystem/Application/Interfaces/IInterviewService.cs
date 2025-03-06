// InterviewSystem/Application/Interfaces/IInterviewService.cs
using InterviewSystem.Application.DTOs;

namespace InterviewSystem.Application.Interfaces;

public interface IInterviewService
{
    Task<IEnumerable<InterviewDto>> GetAllInterviewsAsync();
    Task<IEnumerable<InterviewDto>> GetInterviewsByCompanyAsync(Guid firmId);
    Task<InterviewDto?> GetInterviewByIdAsync(Guid id);
    Task<InterviewDto> CreateInterviewAsync(InterviewDto interviewDto);
    Task UpdateInterviewAsync(InterviewDto interviewDto);
    Task DeleteInterviewAsync(Guid id);
    Task<IEnumerable<InterviewDto>> GetActiveInterviewsAsync();
    Task<IEnumerable<InterviewDto>> GetPassiveInterviewsAsync();
    Task<IEnumerable<InterviewDto>> GetDeletedInterviewsAsync();
}