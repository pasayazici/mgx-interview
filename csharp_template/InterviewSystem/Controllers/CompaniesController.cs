// Controllers/CompaniesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InterviewSystem.Models;
using InterviewSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace InterviewSystem.Controllers;

[Authorize]
public class CompaniesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(ApplicationDbContext context, ILogger<CompaniesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Companies.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Company company)
    {
        if (ModelState.IsValid)
        {
            _context.Add(company);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Company created: {CompanyName}", company.Name);
            return RedirectToAction(nameof(Index));
        }
        return View(company);
    }
}