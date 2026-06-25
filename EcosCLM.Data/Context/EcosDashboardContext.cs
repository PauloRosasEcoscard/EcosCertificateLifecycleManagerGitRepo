using EcosCLM.Data.Configurations;
using EcosCLM.Domain.Entities.Base;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcosCLM.Data.Context
{
    public class EcosDashboardContext : DbContext, IDataProtectionKeyContext
    {
        public DbSet<AuditLogs> AuditLogs { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<PolicySettings> PolicySettings { get; set; }
        public DbSet<SyslogServers> SyslogServers { get; set; }
        public DbSet<DownloadJobs> DownloadJobs { get; set; }

        //Persisting Anti Forgery Tokens and Session state
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<SessionEntry> SessionEntry { get; set; }

        public EcosDashboardContext(DbContextOptions<EcosDashboardContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
