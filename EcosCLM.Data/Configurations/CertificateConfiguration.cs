using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.ToTable("Certificate");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.RequestId);
            builder.Property(x => x.ApplicationId);
            builder.Property(x => x.DomainId);
            builder.Property(x => x.CaId);
            builder.Property(x => x.HsmKeyRefId);
            builder.Property(x => x.PreviousCertificateId);
            builder.Property(x => x.SerialNumber).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ThumbprintSha256).IsRequired().HasMaxLength(64);
            builder.Property(x => x.SubjectDn).IsRequired().HasMaxLength(500);
            builder.Property(x => x.IssuerDn).IsRequired().HasMaxLength(500);
            builder.Property(x => x.NotBefore).IsRequired();
            builder.Property(x => x.NotAfter).IsRequired();
            builder.Property(x => x.CertificatePem).IsRequired();
            builder.Property(x => x.ChainPem);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("ISSUED");
            builder.Property(x => x.RevocationReason).HasMaxLength(255);
            builder.Property(x => x.RevokedAt);
            builder.Property(x => x.InstalledAt);
            builder.Property(x => x.MetadataJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasOne(x => x.Request)
                   .WithMany()
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PreviousCertificate)
                   .WithMany()
                   .HasForeignKey(x => x.PreviousCertificateId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}