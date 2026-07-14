using EcosCLM.Application.Interfaces;
using EcosCLM.Application.Services;
using EcosCLM.Domain.Entities.Base;
using EcosCLM.Domain.Entities.Catalog;
using EcosCLM.Domain.Entities.Certificates;
using EcosCLM.Domain.Entities.Deployment;
using EcosCLM.Domain.Entities.Integration;
using EcosCLM.Domain.Entities.Security;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;

namespace EcosCLM.Data.Context
{
    public class EcosCLMContext : DbContext, IDataProtectionKeyContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider? _serviceProvider;

        public DbSet<AuditLogs> AuditLogs { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<PolicySettings> PolicySettings { get; set; }
        public DbSet<SyslogServers> SyslogServers { get; set; }
        public DbSet<DownloadJobs> DownloadJobs { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<SessionEntry> SessionEntry { get; set; }

        // CLM Domain DbSets
        public DbSet<CLMApplication> CLMApplications { get; set; }
        public DbSet<DeploymentEnvironment> DeploymentEnvironments { get; set; }
        public DbSet<ManagedDomain> ManagedDomains { get; set; }
        public DbSet<ApprovalTask> ApprovalTasks { get; set; }
        public DbSet<CaOrder> CaOrders { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateRequest> CertificateRequests { get; set; }
        public DbSet<CertificateRequestSanDns> CertificateRequestSanDns { get; set; }
        public DbSet<CertificateRequestSanIp> CertificateRequestSanIps { get; set; }
        public DbSet<RenewalJob> RenewalJobs { get; set; }
        public DbSet<CertificateDeployment> CertificateDeployments { get; set; }
        public DbSet<DeploymentTarget> DeploymentTargets { get; set; }
        public DbSet<ApiIdempotencyKey> ApiIdempotencyKeys { get; set; }
        public DbSet<EventOutbox> EventOutboxes { get; set; }
        public DbSet<CertificateAuthority> CertificateAuthorities { get; set; }
        public DbSet<CertificateProfile> CertificateProfiles { get; set; }
        public DbSet<HsmCluster> HsmClusters { get; set; }
        public DbSet<HsmKeyRef> HsmKeyRefs { get; set; }

        public EcosCLMContext(
            DbContextOptions<EcosCLMContext> options,
            IHttpContextAccessor? httpContextAccessor = null,
            IServiceProvider? serviceProvider = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcosCLMContext).Assembly);
        }

        public override int SaveChanges()
        {
            var auditEntries = CreateAuditEntries();

            if (auditEntries.Any())
            {
                AuditLogs.AddRange(auditEntries);

                // Disparo síncrono controlado (safeguard)
                _ = DispatchSyslogEntriesAsync(auditEntries);
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = CreateAuditEntries();

            if (auditEntries.Any())
            {
                await AuditLogs.AddRangeAsync(auditEntries, cancellationToken);

                // Disparo assíncrono correto no pipeline
                await DispatchSyslogEntriesAsync(auditEntries);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private List<AuditLogs> CreateAuditEntries()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditLogs>();

            var httpContext = _httpContextAccessor?.HttpContext;
            var userEmail = httpContext?.User?.Identity?.Name ?? "System";

            var customerIdClaim = httpContext?.User?.FindFirst("CustomerId")?.Value;
            Guid.TryParse(customerIdClaim, out Guid customerId);

            string sourceIp = httpContext?.Connection.RemoteIpAddress?.ToString() == "::1"
                ? "127.0.0.1"
                : httpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            string destIp = httpContext?.Connection.LocalIpAddress?.ToString() == "::1"
                ? "127.0.0.1"
                : httpContext?.Connection.LocalIpAddress?.ToString() ?? "Unknown";

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLogs || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var entityName = entry.Entity.GetType().Name;
                var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue;

                var log = new AuditLogs
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    User = userEmail,
                    IdCustumer = customerId,
                    SourceIp = sourceIp,
                    DestinationIp = destIp,
                    LogType = entry.State.ToString(),
                    Log = $"Entity {entityName} with Key {primaryKey} was {entry.State.ToString().ToLower()}."
                };

                log.Hash = GenerateAuditHash(log);
                auditEntries.Add(log);
            }

            return auditEntries;
        }

        private async Task DispatchSyslogEntriesAsync(List<AuditLogs> entries)
        {
            if (_serviceProvider == null) return;

            try
            {
                var syslogService = _serviceProvider.GetService<ISyslogService>();
                if (syslogService != null)
                {
                    foreach (var log in entries)
                    {
                        // await adicionado corretamente para a nova assinatura assíncrona
                        await syslogService.InitializeAsync(log.IdCustumer);
                        syslogService.SendLog("Ecos Dashboard", log, SyslogSeverity.Information);
                    }
                }
            }
            catch
            {
                // Ignora falhas de rede no Syslog para não quebrar a persistência principal
            }
        }

        private string GenerateAuditHash(AuditLogs entity)
        {
            StringBuilder hashData = new StringBuilder();
            hashData.Append(entity.Date.ToString("o"));
            hashData.Append(entity.User);
            hashData.Append(entity.IdCustumer);
            hashData.Append(entity.LogType);
            hashData.Append(entity.Log);
            hashData.Append(entity.SourceIp);
            hashData.Append(entity.DestinationIp);

            using SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(hashData.ToString());
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}