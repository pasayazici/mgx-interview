// Data/DatabaseSeeder.cs
using Microsoft.AspNetCore.Identity;
using InterviewSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InterviewSystem.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        string[] roles = { "Admin", "User" };
        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user if it doesn't exist
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                Role = "Admin"
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed companies if none exist
        if (!await context.Companies.AnyAsync())
        {
            var companies = new List<Company>
            {
                new Company { Name = "Tech Corp", Industry = "Technology", Location = "San Francisco" },
                new Company { Name = "Finance Plus", Industry = "Finance", Location = "New York" },
                new Company { Name = "Health Care Pro", Industry = "Healthcare", Location = "Boston" },
                new Company { Name = "Marketing Masters", Industry = "Marketing", Location = "Chicago" },
                new Company { Name = "Data Analytics Inc", Industry = "Technology", Location = "Seattle" }
            };
            await context.Companies.AddRangeAsync(companies);
            await context.SaveChangesAsync();

            // Seed interviews
            foreach (var company in companies)
            {
                var interviews = new List<Interview>
                {
                    new Interview 
                    { 
                        CompanyId = company.Id,
                        Position = $"Senior Developer - {company.Name}",
                        ScheduledDate = DateTime.UtcNow.AddDays(7),
                        Status = "Scheduled"
                    },
                    new Interview 
                    { 
                        CompanyId = company.Id,
                        Position = $"Product Manager - {company.Name}",
                        ScheduledDate = DateTime.UtcNow.AddDays(14),
                        Status = "Scheduled"
                    }
                };
                await context.Interviews.AddRangeAsync(interviews);
                await context.SaveChangesAsync();

                // Seed candidates for each interview
                foreach (var interview in interviews)
                {
                    var candidates = new List<Candidate>();
                    for (int i = 1; i <= 4; i++)
                    {
                        candidates.Add(new Candidate
                        {
                            InterviewId = interview.Id,
                            FirstName = $"Candidate{i}",
                            LastName = $"For{interview.Position}",
                            Email = $"candidate{i}.{interview.Position.Replace(" ", "").ToLower()}@example.com",
                            Phone = $"555-0{i}00",
                            Status = "Applied",
                            ResumeUrl = $"/resumes/candidate{i}_{interview.Position.Replace(" ", "_").ToLower()}.pdf"
                        });
                    }
                    await context.Candidates.AddRangeAsync(candidates);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}