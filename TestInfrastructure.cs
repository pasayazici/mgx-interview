using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace InterviewSystem.Tests
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure test database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Configure Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add other required services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IInterviewService, InterviewService>();
            services.AddScoped<ICandidateService, CandidateService>();
        }
    }

    public static class TestHelper
    {
        public static async Task<ApplicationUser> CreateTestUser(
            UserManager<ApplicationUser> userManager,
            string username,
            string role)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = $"{username}@example.com"
            };

            await userManager.CreateAsync(user, "Test@123");
            await userManager.AddToRoleAsync(user, role);

            return user;
        }

        public static async Task<Company> CreateTestCompany(
            ApplicationDbContext context)
        {
            var company = new Company
            {
                Name = "Test Company",
                Description = "Test Description",
                CreatedAt = DateTime.UtcNow
            };

            context.Companies.Add(company);
            await context.SaveChangesAsync();

            return company;
        }

        public static async Task<Interview> CreateTestInterview(
            ApplicationDbContext context)
        {
            var interview = new Interview
            {
                Title = "Test Interview",
                Question = "Test Question",
                Details = "Test Details",
                DurationInSeconds = 180,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow
            };

            context.Interviews.Add(interview);
            await context.SaveChangesAsync();

            return interview;
        }
    }
}
