using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Certificates;
using EcosCLM.Tests.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EcosCLM.Tests.Integration.Repositories.Certificates
{
    [Collection("DatabaseCollection")]
    public class RenewalJobRepositoryTests
    {
        private readonly DatabaseFixture _fixture;

        public RenewalJobRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        private EcosCLMContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<EcosCLMContext>()
                .UseSqlServer(_fixture.ConnectionString)
                .Options;

            return new EcosCLMContext(options);
        }

        [Fact]
        public async Task Should_Filter_RenewalJobs_By_Status_And_Client()
        {
            using var context = CreateContext();
            var customerId = Guid.NewGuid();

            var jobs = new List<RenewalJob>
            {
                new() { Id = Guid.NewGuid(), CustomerId = customerId, Status = "PENDING", ScheduledAt = DateTime.UtcNow, DueAt = DateTime.UtcNow.AddDays(10), CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), CustomerId = customerId, Status = "COMPLETED", ScheduledAt = DateTime.UtcNow, DueAt = DateTime.UtcNow.AddDays(10), CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Status = "PENDING", ScheduledAt = DateTime.UtcNow, DueAt = DateTime.UtcNow.AddDays(10), CreatedAt = DateTime.UtcNow } // Outro cliente
            };

            context.RenewalJobs.AddRange(jobs);
            await context.SaveChangesAsync();

            // Simula a lógica de query que seria chamada pelos Extension Methods do repositório
            var filteredJobs = await context.RenewalJobs
                .Where(x => x.CustomerId == customerId && x.Status == "PENDING")
                .ToListAsync();

            Assert.Single(filteredJobs);
            Assert.Equal("PENDING", filteredJobs[0].Status);
            Assert.Equal(customerId, filteredJobs[0].CustomerId);
        }
    }
}