// InterviewSystem/Application/Services/CandidateService.cs
using AutoMapper;
using InterviewSystem.Application.DTOs;
using InterviewSystem.Application.Interfaces;
using InterviewSystem.Domain.Entities;
using InterviewSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UUID7.NET;

namespace InterviewSystem.Application.Services;

public class CandidateService : ICandidateService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CandidateService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync()
    {
        var candidates = await _context.Candidates
            .Include(c => c.Interview)
            .ThenInclude(i => i.Company)
            .ToListAsync();
        return _mapper.Map<IEnumerable<CandidateDto>>(candidates);
    }

    public async Task<CandidateDto?> GetCandidateByIdAsync(string id)
    {
        var candidate = await _context.Candidates
            .Include(c => c.Interview)
            .FirstOrDefaultAsync(c => c.Id.ToString() == id);
        return candidate != null ? _mapper.Map<CandidateDto>(candidate) : null;
    }

    public async Task<CandidateDto> CreateCandidateAsync(CandidateDto candidateDto)
    {
        var candidate = _mapper.Map<Candidate>(candidateDto);
        candidate.Id = Guid.Parse(Uuid7.NewString());
        candidate.RegistrationDate = DateTime.UtcNow;
        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();
        return _mapper.Map<CandidateDto>(candidate);
    }

    public async Task<IEnumerable<CandidateDto>> CreateBulkCandidatesAsync(Guid interviewId, IEnumerable<string> emails)
    {
        var uniqueEmails = emails.Distinct();
        var existingEmails = await _context.Candidates
            .Where(c => c.InterviewId == interviewId && uniqueEmails.Contains(c.Email))
            .Select(c => c.Email)
            .ToListAsync();

        var newCandidates = uniqueEmails
            .Except(existingEmails)
            .Select(email => new Candidate
            {
                Id = Guid.Parse(Uuid7.NewString()),
                Email = email,
                InterviewId = interviewId,
                RegistrationDate = DateTime.UtcNow,
                Status = InterviewStatus.Pending
            });

        _context.Candidates.AddRange(newCandidates);
        await _context.SaveChangesAsync();
        return _mapper.Map<IEnumerable<CandidateDto>>(newCandidates);
    }

    public async Task UpdateCandidateAsync(CandidateDto candidateDto)
    {
        var candidate = await _context.Candidates.FindAsync(Guid.Parse(candidateDto.Id!));
        if (candidate == null)
        {
            throw new InvalidOperationException("Candidate not found");
        }

        _mapper.Map(candidateDto, candidate);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCandidateAsync(string id)
    {
        var candidate = await _context.Candidates.FindAsync(Guid.Parse(id));
        if (candidate == null)
        {
            throw new InvalidOperationException("Candidate not found");
        }

        candidate.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task RunAnalysisAsync(IEnumerable<string> candidateIds)
    {
        var candidates = await _context.Candidates
            .Where(c => candidateIds.Contains(c.Id.ToString()))
            .ToListAsync();

        foreach (var candidate in candidates)
        {
            candidate.Status = InterviewStatus.Analyzed;
            // TODO: Implement actual video analysis logic
        }

        await _context.SaveChangesAsync();
    }

    public async Task RefreshStatusAsync(IEnumerable<string> candidateIds)
    {
        var candidates = await _context.Candidates
            .Where(c => candidateIds.Contains(c.Id.ToString()))
            .ToListAsync();

        foreach (var candidate in candidates)
        {
            // TODO: Implement actual status refresh logic
            if (candidate.VideoUrl != null)
            {
                candidate.Status = InterviewStatus.Completed;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateFeedbackAsync(string id, int score, string feedback)
    {
        var candidate = await _context.Candidates.FindAsync(Guid.Parse(id));
        if (candidate == null)
        {
            throw new InvalidOperationException("Candidate not found");
        }

        candidate.Score = score;
        candidate.Feedback = feedback;
        await _context.SaveChangesAsync();
    }
}