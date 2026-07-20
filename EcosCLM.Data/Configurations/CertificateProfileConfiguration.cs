using EcosCLM.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class CertificateProfileConfiguration : IEntityTypeConfiguration<CertificateProfile>
    {
        public void Configure(EntityTypeBuilder<CertificateProfile> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ToTable("CertificateProfile");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.CertificateType).IsRequired().HasMaxLength(100).HasDefaultValue("TLS_SERVER");
            builder.Property(x => x.KeyAlgorithm).IsRequired().HasMaxLength(50).HasDefaultValue("RSA");
            builder.Property(x => x.KeySize);
            builder.Property(x => x.CurveName).HasMaxLength(50);
            builder.Property(x => x.SignatureAlgorithm).HasMaxLength(100);
            builder.Property(x => x.ValidityDays).IsRequired();
            builder.Property(x => x.RenewalWindowDays).IsRequired().HasDefaultValue(30);
            builder.Property(x => x.SubjectTemplateJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.SanPolicyJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.RequireApproval).IsRequired().HasDefaultValue((short)1);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("ACTIVE");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}