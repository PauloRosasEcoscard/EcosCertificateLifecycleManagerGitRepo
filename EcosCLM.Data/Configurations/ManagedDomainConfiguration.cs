using EcosCLM.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class ManagedDomainConfiguration : IEntityTypeConfiguration<ManagedDomain>
    {
        public void Configure(EntityTypeBuilder<ManagedDomain> builder)
        {
            builder.ToTable("ManagedDomain");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.ApplicationId);
            builder.Property(x => x.Fqdn).IsRequired().HasMaxLength(255);
            builder.Property(x => x.ValidationMethod).IsRequired().HasMaxLength(50).HasDefaultValue("DNS");
            builder.Property(x => x.ValidationStatus).IsRequired().HasMaxLength(50).HasDefaultValue("PENDING");
            builder.Property(x => x.ValidatedAt);
            builder.Property(x => x.ExpiresAt);
            builder.Property(x => x.MetadataJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasOne(x => x.Application)
                   .WithMany(x => x.Domains)
                   .HasForeignKey(x => x.ApplicationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}