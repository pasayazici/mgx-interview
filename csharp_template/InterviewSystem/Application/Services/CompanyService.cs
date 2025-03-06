// InterviewSystem/Application/Services/CompanyService.cs
using AutoMapper;
using InterviewSystem.Application.DTOs;
using InterviewSystem.Application.Interfaces;
using InterviewSystem.Domain.Entities;
using InterviewSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InterviewSystem.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CompanyService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
    {
        var companies = await _context.Companies.ToListAsync();
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        return company != null ? _mapper.Map<CompanyDto>(company) : null;
    }

    public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
    {
        var company = _mapper.Map<Company>(companyDto);
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        return _mapper.Map<CompanyDto>(company);
    }

    public async Task UpdateCompanyAsync(CompanyDto companyDto)
    {
        var company = await _context.Companies.FindAsync(companyDto.Id);
        if (company == null)
        {
            throw new InvalidOperationException("Company not found");
        }

        _mapper.Map(companyDto, company);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCompanyAsync(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
        {
            throw new InvalidOperationException("Company not found");
        }

        company.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}