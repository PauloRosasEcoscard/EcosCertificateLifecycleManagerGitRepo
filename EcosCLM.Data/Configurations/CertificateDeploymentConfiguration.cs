using EcosCLM.Domain.Entities.Deployment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class CertificateDeploymentConfiguration : IEntityTypeConfiguration<CertificateDeployment>
    {
        public void Configure(EntityTypeBuilder<CertificateDeployment> builder)
        {
            builder.ToTable("CertificateDeployment");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.CertificateId).IsRequired();
            builder.Property(x => x.TargetId).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("PENDING");
            builder.Property(x => x.DeployedAt);
            builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasOne(x => x.Target)
                   .WithMany()
                   .HasForeignKey(x => x.TargetId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}