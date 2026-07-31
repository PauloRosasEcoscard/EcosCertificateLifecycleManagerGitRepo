using EcosCLM.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosCLM.Data.Extensions
{
    public static class DatabaseInitializationExtensions
    {
        public static async Task MigrateAndSeedEcosCLMAsync(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var services = serviceScope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<EcosCLMContext>>();

            try
            {
                var context = services.GetRequiredService<EcosCLMContext>();

                logger.LogInformation("Applying pending migrations for EcosCLMContext...");
                await context.Database.MigrateAsync().ConfigureAwait(false);

                logger.LogInformation("Executing initial database seeding...");
                await services.SeedStandardEcosCLMAsync().ConfigureAwait(false);

                logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or seeding the EcosCLM database.");
                throw;
            }
        }

        private static async Task SeedStandardEcosCLMAsync(this IServiceProvider services)
        {
            // Implemente aqui as chamadas de Seeding iniciais (Ex: Perfis, Administrador)
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}