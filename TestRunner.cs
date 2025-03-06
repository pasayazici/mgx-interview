using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewSystem.Tests
{
    [TestFixture]
    public class TestRunner
    {
        private static List<TestResult> _testResults = new List<TestResult>();

        public class TestResult
        {
            public string TestName { get; set; }
            public string Category { get; set; }
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public TimeSpan Duration { get; set; }
        }

        [OneTimeSetUp]
        public async Task InitializeTestEnvironment()
        {
            try
            {
                // Setup test environment
                var webHostBuilder = new WebHostBuilder()
                    .UseStartup<TestStartup>();
                using var server = new TestServer(webHostBuilder);
                using var scope = server.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Setup initial test data
                await SetupInitialTestData(context);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test environment initialization failed: {ex.Message}");
            }
        }

        private async Task SetupInitialTestData(ApplicationDbContext context)
        {
            var userManager = context.GetService<UserManager<ApplicationUser>>();

            // Create default SuperAdmin
            await TestHelper.CreateTestUser(userManager, "admin", "SuperAdmin");

            // Create test company
            await TestHelper.CreateTestCompany(context);

            // Create test interview
            await TestHelper.CreateTestInterview(context);
        }

        [Test, Order(1)]
        public async Task RunAllTests()
        {
            var testFixtures = new List<object>
            {
                new AuthenticationTests(),
                new CompanyManagementTests(),
                new InterviewManagementTests(),
                new CandidateManagementTests()
            };

            foreach (var fixture in testFixtures)
            {
                var fixtureType = fixture.GetType();
                var tests = fixtureType.GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any());

                foreach (var test in tests)
                {
                    var result = new TestResult
                    {
                        TestName = test.Name,
                        Category = fixtureType.Name
                    };

                    var startTime = DateTime.Now;

                    try
                    {
                        await (Task)test.Invoke(fixture, null);
                        result.Success = true;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.ErrorMessage = ex.Message;
                    }

                    result.Duration = DateTime.Now - startTime;
                    _testResults.Add(result);
                }
            }

            // Generate test report
            GenerateTestReport();
        }

        private void GenerateTestReport()
        {
            var totalTests = _testResults.Count;
            var passedTests = _testResults.Count(r => r.Success);
            var failedTests = totalTests - passedTests;

            Console.WriteLine("\nTest Execution Report");
            Console.WriteLine("====================");
            Console.WriteLine($"Total Tests: {totalTests}");
            Console.WriteLine($"Passed: {passedTests}");
            Console.WriteLine($"Failed: {failedTests}");
            Console.WriteLine($"Success Rate: {(passedTests * 100.0 / totalTests):F2}%\n");

            foreach (var category in _testResults.Select(r => r.Category).Distinct())
            {
                Console.WriteLine($"\n{category}");
                Console.WriteLine(new string('-', category.Length));

                var categoryTests = _testResults.Where(r => r.Category == category);
                foreach (var test in categoryTests)
                {
                    Console.WriteLine($"{test.TestName}: {(test.Success ? "✓" : "✗")} "
                        + $"({test.Duration.TotalSeconds:F2}s)");
                    if (!test.Success)
                    {
                        Console.WriteLine($"  Error: {test.ErrorMessage}");
                    }
                }
            }
        }
    }
}
