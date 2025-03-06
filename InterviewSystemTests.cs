using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InterviewSystem.Tests
{
    [TestFixture]
    public class AuthenticationTests
    {
        private TestServer _server;
        private HttpClient _client;
        private ApplicationDbContext _context;

        [OneTimeSetUp]
        public async Task Setup()
        {
            // Setup test server and client
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<TestStartup>();
            _server = new TestServer(webHostBuilder);
            _client = _server.CreateClient();

            // Setup test database
            _context = _server.Host.Services.GetRequiredService<ApplicationDbContext>();
            await _context.Database.MigrateAsync();
        }

        [Test]
        public async Task SuperAdmin_Login_WithDefaultCredentials_ShouldSucceed()
        {
            // Arrange
            var loginModel = new LoginViewModel
            {
                Username = "admin",
                Password = "admin"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Account/Login", loginModel);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(content, Does.Contain("Welcome"));
        }

        [Test]
        public async Task User_Creation_BySuperAdmin_ShouldSucceed()
        {
            // Arrange
            await LoginAsSuperAdmin();
            var newUser = new CreateUserViewModel
            {
                Username = "testadmin",
                Email = "testadmin@example.com",
                Password = "Test@123",
                Role = "Admin",
                FirmId = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync("/User/Create", newUser);

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);
            Assert.That(user, Is.Not.Null);
        }
    }

    [TestFixture]
    public class CompanyManagementTests
    {
        private TestServer _server;
        private HttpClient _client;
        private ApplicationDbContext _context;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<TestStartup>();
            _server = new TestServer(webHostBuilder);
            _client = _server.CreateClient();
            _context = _server.Host.Services.GetRequiredService<ApplicationDbContext>();
        }

        [Test]
        public async Task Company_Creation_BySuperAdmin_ShouldSucceed()
        {
            // Arrange
            await LoginAsSuperAdmin();
            var newCompany = new CreateCompanyViewModel
            {
                Name = "Test Company",
                Description = "Test Description"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Company/Create", newCompany);

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Name == newCompany.Name);
            Assert.That(company, Is.Not.Null);
        }

        [Test]
        public async Task Company_SoftDelete_ShouldUpdateIsDeletedFlag()
        {
            // Arrange
            await LoginAsSuperAdmin();
            var company = await CreateTestCompany();

            // Act
            var response = await _client.DeleteAsync($"/Company/Delete/{company.Id}");

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var deletedCompany = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == company.Id);
            Assert.That(deletedCompany.IsDeleted, Is.True);
        }
    }

    [TestFixture]
    public class InterviewManagementTests
    {
        private TestServer _server;
        private HttpClient _client;
        private ApplicationDbContext _context;

        [Test]
        public async Task Interview_Creation_WithValidData_ShouldSucceed()
        {
            // Arrange
            var newInterview = new CreateInterviewViewModel
            {
                Title = "Test Interview",
                Question = "Test Question",
                Details = "Test Details",
                DurationInSeconds = 180,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Interview/Create", newInterview);

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.Title == newInterview.Title);
            Assert.That(interview, Is.Not.Null);
        }
    }

    [TestFixture]
    public class CandidateManagementTests
    {
        private TestServer _server;
        private HttpClient _client;
        private ApplicationDbContext _context;

        [Test]
        public async Task Candidate_BulkRegistration_ShouldHandleDuplicates()
        {
            // Arrange
            var emails = "test1@example.com\ntest1@example.com\ntest2@example.com";
            var interviewId = await CreateTestInterview();

            var model = new BulkCandidateRegistrationViewModel
            {
                InterviewId = interviewId,
                Emails = emails
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Candidate/BulkRegister", model);

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var candidates = await _context.Candidates
                .Where(c => c.InterviewId == interviewId)
                .ToListAsync();
            Assert.That(candidates.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Candidate_Registration_ShouldGenerateValidUUID()
        {
            // Arrange
            var email = "test@example.com";
            var interviewId = await CreateTestInterview();

            // Act
            var response = await _client.PostAsJsonAsync("/Candidate/Register", 
                new { InterviewId = interviewId, Email = email });

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == email);
            Assert.That(candidate, Is.Not.Null);
            Assert.That(Guid.TryParse(candidate.Id, out _), Is.True);
        }
    }
}
