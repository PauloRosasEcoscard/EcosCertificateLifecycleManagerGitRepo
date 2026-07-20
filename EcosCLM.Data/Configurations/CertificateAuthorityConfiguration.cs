using EcosCLM.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class CertificateAuthorityConfiguration : IEntityTypeConfiguration<CertificateAuthority>
    {
        public void Configure(EntityTypeBuilder<CertificateAuthority> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ToTable("CertificateAuthority");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ProviderType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.BaseUrl).HasMaxLength(500);
            builder.Property(x => x.AccountRef).HasMaxLength(255);
            builder.Property(x => x.SupportsAcme).IsRequired().HasDefaultValue((short)0);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("ACTIVE");
            builder.Property(x => x.MetadataJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}