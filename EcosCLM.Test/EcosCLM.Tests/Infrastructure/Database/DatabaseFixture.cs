using EcosCLM.Data.Context;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace EcosCLM.Tests.Infrastructure.Database
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        public string ConnectionString => _dbContainer.GetConnectionString();

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            var options = new DbContextOptionsBuilder<EcosCLMContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            using var context = new EcosCLMContext(options);
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
        }
    }

    [CollectionDefinition("DatabaseCollection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}