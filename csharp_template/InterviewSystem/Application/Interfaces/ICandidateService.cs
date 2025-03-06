// InterviewSystem/Application/Interfaces/ICandidateService.cs
using InterviewSystem.Application.DTOs;

namespace InterviewSystem.Application.Interfaces;

public interface ICandidateService
{
    Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync();
    Task<CandidateDto?> GetCandidateByIdAsync(string id);
    Task<CandidateDto> CreateCandidateAsync(CandidateDto candidateDto);
    Task<IEnumerable<CandidateDto>> CreateBulkCandidatesAsync(Guid interviewId, IEnumerable<string> emails);
    Task UpdateCandidateAsync(CandidateDto candidateDto);
    Task DeleteCandidateAsync(string id);
    Task RunAnalysisAsync(IEnumerable<string> candidateIds);
    Task RefreshStatusAsync(IEnumerable<string> candidateIds);
    Task UpdateFeedbackAsync(string id, int score, string feedback);
}