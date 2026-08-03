using EcosCLM.Data.Context;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xunit;

namespace EcosCLM.Tests.Infrastructure.Database
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly MySqlContainer _dbContainer = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .Build();

        public string ConnectionString => _dbContainer.GetConnectionString();

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync().ConfigureAwait(false);

            var options = new DbContextOptionsBuilder<EcosCLMContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            using var context = new EcosCLMContext(options);
            await context.Database.MigrateAsync().ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync().ConfigureAwait(false);
            await _dbContainer.DisposeAsync().ConfigureAwait(false);
        }
    }

    [CollectionDefinition("DatabaseCollection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}