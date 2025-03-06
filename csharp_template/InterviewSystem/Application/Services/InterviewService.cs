// InterviewSystem/Application/Services/InterviewService.cs
using AutoMapper;
using InterviewSystem.Application.DTOs;
using InterviewSystem.Application.Interfaces;
using InterviewSystem.Domain.Entities;
using InterviewSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InterviewSystem.Application.Services;

public class InterviewService : IInterviewService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public InterviewService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InterviewDto>> GetAllInterviewsAsync()
    {
        var interviews = await _context.Interviews
            .Include(i => i.Company)
            .ToListAsync();
        return _mapper.Map<IEnumerable<InterviewDto>>(interviews);
    }

    public async Task<IEnumerable<InterviewDto>> GetInterviewsByCompanyAsync(Guid firmId)
    {
        var interviews = await _context.Interviews
            .Include(i => i.Company)
            .Where(i => i.FirmId == firmId)
            .ToListAsync();
        return _mapper.Map<IEnumerable<InterviewDto>>(interviews);
    }

    public async Task<InterviewDto?> GetInterviewByIdAsync(Guid id)
    {
        var interview = await _context.Interviews
            .Include(i => i.Company)
            .FirstOrDefaultAsync(i => i.Id == id);
        return interview != null ? _mapper.Map<InterviewDto>(interview) : null;
    }

    public async Task<InterviewDto> CreateInterviewAsync(InterviewDto interviewDto)
    {
        var interview = _mapper.Map<Interview>(interviewDto);
        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync();
        return _mapper.Map<InterviewDto>(interview);
    }

    public async Task UpdateInterviewAsync(InterviewDto interviewDto)
    {
        var interview = await _context.Interviews.FindAsync(interviewDto.Id);
        if (interview == null)
        {
            throw new InvalidOperationException("Interview not found");
        }

        _mapper.Map(interviewDto, interview);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteInterviewAsync(Guid id)
    {
        var interview = await _context.Interviews.FindAsync(id);
        if (interview == null)
        {
            throw new InvalidOperationException("Interview not found");
        }

        interview.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<InterviewDto>> GetActiveInterviewsAsync()
    {
        var now = DateTime.UtcNow;
        var interviews = await _context.Interviews
            .Include(i => i.Company)
            .Where(i => i.StartDate <= now && i.EndDate >= now)
            .ToListAsync();
        return _mapper.Map<IEnumerable<InterviewDto>>(interviews);
    }

    public async Task<IEnumerable<InterviewDto>> GetPassiveInterviewsAsync()
    {
        var now = DateTime.UtcNow;
        var interviews = await _context.Interviews
            .Include(i => i.Company)
            .Where(i => i.StartDate > now || i.EndDate < now)
            .ToListAsync();
        return _mapper.Map<IEnumerable<InterviewDto>>(interviews);
    }

    public async Task<IEnumerable<InterviewDto>> GetDeletedInterviewsAsync()
    {
        var interviews = await _context.Interviews
            .Include(i => i.Company)
            .IgnoreQueryFilters()
            .Where(i => i.IsDeleted)
            .ToListAsync();
        return _mapper.Map<IEnumerable<InterviewDto>>(interviews);
    }
}