using EcosCLM.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    class SessionEntryConfiguration : IEntityTypeConfiguration<SessionEntry>
    {
        public void Configure(EntityTypeBuilder<SessionEntry> builder)
        {
            builder.ToTable("SessionEntry");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id);

            builder.Property(e => e.Value);
            builder.Property(e => e.ExpiresAtTime);
            builder.Property(e => e.SlidingExpirationInSeconds);
            builder.Property(e => e.AbsoluteExpiration);
        }
    }
}
