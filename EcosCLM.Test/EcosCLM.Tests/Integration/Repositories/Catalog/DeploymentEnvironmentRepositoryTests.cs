using EcosCLM.Data.Context;
using EcosCLM.Domain.Entities.Catalog;
using EcosCLM.Tests.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EcosCLM.Tests.Integration.Repositories.Catalog
{
    [Collection("DatabaseCollection")]
    public class DeploymentEnvironmentRepositoryTests
    {
        private readonly DatabaseFixture _fixture;

        public DeploymentEnvironmentRepositoryTests(DatabaseFixture fixture)
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
        public async Task Should_Persist_And_Retrieve_DeploymentEnvironment()
        {
            using var context = CreateContext();

            var environment = new DeploymentEnvironment
            {
                Id = Guid.NewGuid(),
                Code = "STG",
                Name = "Staging Environment",
                CustomerId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            context.DeploymentEnvironments.Add(environment);
            await context.SaveChangesAsync();

            var persisted = await context.DeploymentEnvironments
                .FirstOrDefaultAsync(x => x.Code == "STG");

            Assert.NotNull(persisted);
            Assert.Equal("Staging Environment", persisted.Name);
        }
    }
}