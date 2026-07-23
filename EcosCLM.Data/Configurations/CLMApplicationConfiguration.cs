using EcosCLM.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class CLMApplicationConfiguration : IEntityTypeConfiguration<CLMApplication>
    {
        public void Configure(EntityTypeBuilder<CLMApplication> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ToTable("CLMApplication");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.OwnerUserId);
            builder.Property(x => x.Criticality).IsRequired().HasMaxLength(50).HasDefaultValue("MEDIUM");
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("ACTIVE");
            builder.Property(x => x.MetadataJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasMany(x => x.Domains)
                   .WithOne(x => x.Application)
                   .HasForeignKey(x => x.ApplicationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}