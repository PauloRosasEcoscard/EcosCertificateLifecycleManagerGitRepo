using EcosCLM.Domain.Entities.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class EventOutboxConfiguration : IEntityTypeConfiguration<EventOutbox>
    {
        public void Configure(EntityTypeBuilder<EventOutbox> builder)
        {
            builder.ToTable("EventOutbox");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventType).IsRequired().HasMaxLength(150);
            builder.Property(x => x.PayloadJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("PENDING");
            builder.Property(x => x.Retries).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}