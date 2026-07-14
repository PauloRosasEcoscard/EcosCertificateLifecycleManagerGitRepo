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
                services.AddDbContext<EcosCLMContext>(opts =>
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
                services.AddDbContext<EcosCLMContext>(opts =>
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
            services.AddTransient<EcosCLMContext>();
            services.AddScoped<IAuditLogsRepository, AuditLogsRepository>();
            services.AddScoped<ISyslogServersRepository, SyslogServersRepository>();
            services.AddScoped<INotificationsRepository, NotificationsRepository>();
            services.AddScoped<IPolicySettingsRepository, PolicySettingsRepository>();
            services.AddScoped<IDownloadJobsRepository, DownloadJobsRepository>();
            services.AddScoped<ISessionEntryRepository, SessionEntryRepository>();
            services.AddScoped<ISyslogService, SyslogService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();

            services.AddScoped<ICLMApplicationRepository, CLMApplicationRepository>();
            services.AddScoped<IDeploymentEnvironmentRepository, DeploymentEnvironmentRepository>();
            services.AddScoped<IManagedDomainRepository, ManagedDomainRepository>();
            services.AddScoped<ICertificateAuthorityRepository, CertificateAuthorityRepository>();
            services.AddScoped<ICertificateProfileRepository, CertificateProfileRepository>();
            services.AddScoped<IHsmClusterRepository, HsmClusterRepository>();
            services.AddScoped<IHsmKeyRefRepository, HsmKeyRefRepository>();
            services.AddScoped<ICertificateDeploymentRepository, CertificateDeploymentRepository>();
            services.AddScoped<IDeploymentTargetRepository, DeploymentTargetRepository>();
            services.AddScoped<IApiIdempotencyKeyRepository, ApiIdempotencyKeyRepository>();
            services.AddScoped<IEventOutboxRepository, EventOutboxRepository>();
            services.AddScoped<IApprovalTaskRepository, ApprovalTaskRepository>();
            services.AddScoped<ICaOrderRepository, CaOrderRepository>();
            services.AddScoped<ICertificateRepository, CertificateRepository>();
            services.AddScoped<ICertificateRequestRepository, CertificateRequestRepository>();
            services.AddScoped<ICertificateRequestSanDnsRepository, CertificateRequestSanDnsRepository>();
            services.AddScoped<IRenewalJobRepository, RenewalJobRepository>();
            #endregion
        }

    }
}
