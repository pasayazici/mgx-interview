// Controllers/CandidatesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InterviewSystem.Models;
using InterviewSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace InterviewSystem.Controllers;

[Authorize]
public class CandidatesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CandidatesController> _logger;

    public CandidatesController(ApplicationDbContext context, ILogger<CandidatesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Candidates
            .Include(c => c.Interview)
            .ThenInclude(i => i.Company)
            .ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.Interviews = new SelectList(_context.Interviews, "Id", "Position");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Candidate candidate)
    {
        if (ModelState.IsValid)
        {
            _context.Add(candidate);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Candidate created: {CandidateName}", $"{candidate.FirstName} {candidate.LastName}");
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Interviews = new SelectList(_context.Interviews, "Id", "Position");
        return View(candidate);
    }
}