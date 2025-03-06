// InterviewSystem/Application/Interfaces/ICompanyService.cs
using InterviewSystem.Application.DTOs;

namespace InterviewSystem.Application.Interfaces;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
    Task<CompanyDto?> GetCompanyByIdAsync(Guid id);
    Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
    Task UpdateCompanyAsync(CompanyDto companyDto);
    Task DeleteCompanyAsync(Guid id);
}