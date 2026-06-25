using EcosCLM.Application.Interfaces;
using EcosCLM.Application.Services;
using EcosCLM.Data.Context;
using EcosCLM.Data.Repositories;
using EcosCLM.Data.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.Extensions.Caching.MySql;
using System.Reflection.Emit;

namespace EcosCLM.Data.Extensions
{
    public static class ServiceCollectionsExtension
    {
        public static void ServicesApplication(IServiceCollection services, IConfiguration configuration)
        {
            var repositoryType = configuration.GetSection("RepositoryType")?.Value?.ToUpper();
            var connectionStrings = configuration.GetSection("ConnectionStrings");
            string dashboardConnectionString = configuration.GetConnectionString("EcosCLM")!;

            if (repositoryType == "SQL")
            {
                services.AddDbContext<EcosDashboardContext>(opts =>
                    opts.UseSqlServer(
                        dashboardConnectionString,
                        sql => sql.MigrationsAssembly("EcosCLM.Migrations.SQL")
                    ),
                    ServiceLifetime.Transient);

                services.AddDistributedSqlServerCache(options =>
                {
                    options.ConnectionString = dashboardConnectionString;
                    options.SchemaName = "dbo";
                    options.TableName = "SessionEntry";
                });
            }
            else if (repositoryType == "MYSQL")
            {
                services.AddDbContext<EcosDashboardContext>(opts =>
                    opts.UseMySql(
                        dashboardConnectionString,
                        ServerVersion.AutoDetect(dashboardConnectionString),
                        my => my.MigrationsAssembly("EcosCLM.Migrations.MYSQL")
                    ).LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging(),
                    ServiceLifetime.Transient);

                services.AddDistributedMySqlCache(options =>
                {
                    options.ConnectionString = dashboardConnectionString;
                    options.TableName = "SessionEntry";
                });
            }
            #region Services
            services.AddTransient<EmailService>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue>(_ =>
            {
                if (!int.TryParse(configuration["QueueCapacity"], out var queueCapacity))
                {
                    queueCapacity = 100;
                }

                return new DefaultBackgroundTaskQueue(queueCapacity);
            });
            services.AddSingleton<IDownloadManager, DownloadManager>();
            services.AddScoped<FileGenerator>();
            #endregion

            #region DependencyInjection
            services.AddTransient<EcosDashboardContext>();
            services.AddScoped<IAuditLogsRepository, AuditLogsRepository>();
            services.AddScoped<ISyslogServersRepository, SyslogServersRepository>();
            services.AddScoped<INotificationsRepository, NotificationsRepository>();
            services.AddScoped<IPolicySettingsRepository, PolicySettingsRepository>();
            services.AddScoped<IDownloadJobsRepository, DownloadJobsRepository>();
            services.AddScoped<ISessionEntryRepository, SessionEntryRepository>();
            services.AddScoped<ISyslogService, SyslogService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            #endregion
        }

    }
}
