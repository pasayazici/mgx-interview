using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InterviewSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InterviewSystem.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed roles
            await SeedRoles(roleManager);

            // Seed admin user
            await SeedAdminUser(userManager);

            // Seed companies
            var companies = await SeedCompanies(context);

            // Seed interviews
            var interviews = await SeedInterviews(context, companies);

            // Seed candidates
            await SeedCandidates(context, interviews);

            await context.SaveChangesAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "SuperAdmin", "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                _logger.LogInformation("Created role: {Role}", role);
            }
        }
    }

    private async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = "admin@example.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "SuperAdmin");
                _logger.LogInformation("Created admin user: {Email}", adminEmail);
            }
        }
    }

    private async Task<List<Company>> SeedCompanies(ApplicationDbContext context)
    {
        var companies = new List<Company>
        {
            new() { Name = "Tech Solutions Inc", CreatedAt = DateTime.UtcNow },
            new() { Name = "Digital Innovations Ltd", CreatedAt = DateTime.UtcNow },
            new() { Name = "Future Systems Corp", CreatedAt = DateTime.UtcNow },
            new() { Name = "Smart Software Solutions", CreatedAt = DateTime.UtcNow },
            new() { Name = "Cloud Technologies Group", CreatedAt = DateTime.UtcNow }
        };

        foreach (var company in companies)
        {
            if (!await context.Companies.AnyAsync(c => c.Name == company.Name))
            {
                await context.Companies.AddAsync(company);
                _logger.LogInformation("Created company: {CompanyName}", company.Name);
            }
        }

        await context.SaveChangesAsync();
        return companies;
    }

    private async Task<List<Interview>> SeedInterviews(ApplicationDbContext context, List<Company> companies)
    {
        var interviews = new List<Interview>();
        var random = new Random();

        foreach (var company in companies)
        {
            for (int i = 0; i < 2; i++)
            {
                var interview = new Interview
                {
                    Title = $"Software Developer Position {i + 1}",
                    Question = "Tell us about your experience with C# and .NET",
                    Details = "We are looking for a skilled .NET developer",
                    Duration = 30,
                    StartDate = DateTime.UtcNow.AddDays(random.Next(1, 10)),
                    EndDate = DateTime.UtcNow.AddDays(random.Next(11, 30)),
                    FirmId = company.Id,
                    CreatedAt = DateTime.UtcNow
                };

                interviews.Add(interview);
                await context.Interviews.AddAsync(interview);
                _logger.LogInformation("Created interview: {InterviewTitle} for company {CompanyName}", 
                    interview.Title, company.Name);
            }
        }

        await context.SaveChangesAsync();
        return interviews;
    }

    private async Task SeedCandidates(ApplicationDbContext context, List<Interview> interviews)
    {
        var random = new Random();
        foreach (var interview in interviews)
        {
            for (int i = 0; i < 4; i++)
            {
                var candidate = new Candidate
                {
                    Email = $"candidate{i + 1}_{interview.Id.ToString()[..4]}@example.com",
                    InterviewId = interview.Id,
                    Status = (CandidateStatus)random.Next(0, 5),
                    RegistrationDate = DateTime.UtcNow.AddDays(-random.Next(1, 10)),
                    CreatedAt = DateTime.UtcNow
                };

                await context.Candidates.AddAsync(candidate);
                _logger.LogInformation("Created candidate: {CandidateEmail} for interview {InterviewTitle}",
                    candidate.Email, interview.Title);
            }
        }

        await context.SaveChangesAsync();
    }
}