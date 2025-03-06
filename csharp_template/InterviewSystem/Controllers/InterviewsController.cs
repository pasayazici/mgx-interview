// Controllers/InterviewsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InterviewSystem.Models;
using InterviewSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace InterviewSystem.Controllers;

[Authorize]
public class InterviewsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InterviewsController> _logger;

    public InterviewsController(ApplicationDbContext context, ILogger<InterviewsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Interviews.Include(i => i.Company).ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.Companies = new SelectList(_context.Companies, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Interview interview)
    {
        if (ModelState.IsValid)
        {
            _context.Add(interview);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Interview created for company: {CompanyId}", interview.CompanyId);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Companies = new SelectList(_context.Companies, "Id", "Name");
        return View(interview);
    }
}