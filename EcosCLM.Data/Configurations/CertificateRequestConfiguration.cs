using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class CertificateRequestConfiguration : IEntityTypeConfiguration<CertificateRequest>
    {
        public void Configure(EntityTypeBuilder<CertificateRequest> builder)
        {
            builder.ToTable("CertificateRequest");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.RequestType).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("DRAFT");
            builder.Property(x => x.CertificateRequestCLMApplicationId);
            builder.Property(x => x.CertificateRequestDomainId);
            builder.Property(x => x.CertificateRequestProfileId);
            builder.Property(x => x.CaId);
            builder.Property(x => x.HsmClusterId);
            builder.Property(x => x.HsmKeyRefId);
            builder.Property(x => x.RequestedBy);
            builder.Property(x => x.SubjectDn).IsRequired().HasMaxLength(500);
            builder.Property(x => x.KeyPolicyJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.CsrPem);
            builder.Property(x => x.FailureReason).HasMaxLength(1000);
            builder.Property(x => x.MetadataJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasOne(x => x.CertificateRequestCLMApplication)
                   .WithMany(x => (IEnumerable<CertificateRequest>)x.CLMApplicationCertificateRequests)
                   .HasForeignKey(x => x.CertificateRequestCLMApplicationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CertificateRequestDomain)
                   .WithMany()
                   .HasForeignKey(x => x.CertificateRequestDomainId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CertificateRequestProfile)
                   .WithMany()
                   .HasForeignKey(x => x.CertificateRequestProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CertificateAuthority)
                   .WithMany()
                   .HasForeignKey(x => x.CaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.SanDns)
                   .WithOne(x => x.Request)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.SanIps)
                   .WithOne(x => x.Request)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ApprovalTasks)
                   .WithOne(x => x.Request)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}