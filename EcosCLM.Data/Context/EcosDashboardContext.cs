using EcosCLM.Application.Interfaces;
using EcosCLM.Application.Services;
using EcosCLM.Data.Configurations;
using EcosCLM.Data.Services;
using EcosCLM.Domain.Entities.Base;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EcosCLM.Data.Context
{
    public class EcosDashboardContext : DbContext, IDataProtectionKeyContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISyslogService _syslogService;

        public DbSet<AuditLogs> AuditLogs { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<PolicySettings> PolicySettings { get; set; }
        public DbSet<SyslogServers> SyslogServers { get; set; }
        public DbSet<DownloadJobs> DownloadJobs { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<SessionEntry> SessionEntry { get; set; }

        public EcosDashboardContext(
            DbContextOptions<EcosDashboardContext> options,
            IHttpContextAccessor httpContextAccessor,
            ISyslogService syslogService) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _syslogService = syslogService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcosDashboardContext).Assembly);
        }

        public override int SaveChanges()
        {
            OnBeforeSaveChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditLogs>();

            var httpContext = _httpContextAccessor.HttpContext;
            var userEmail = httpContext?.User?.Identity?.Name ?? "System";

            // Substitua pela lógica real de extração do Tenant/Customer do seu token JWT ou Session
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

                // Disparo opcional e assíncrono para o Syslog
                try
                {
                    _syslogService.Initialize(log.IdCustumer);
                    _syslogService.SendLog("Ecos Dashboard", log, SyslogSeverity.Information);
                }
                catch { }
            }

            if (auditEntries.Any())
            {
                AuditLogs.AddRange(auditEntries);
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