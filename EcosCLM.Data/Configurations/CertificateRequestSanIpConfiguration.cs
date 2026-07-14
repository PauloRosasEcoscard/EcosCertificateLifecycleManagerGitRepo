using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class CertificateRequestSanIpConfiguration : IEntityTypeConfiguration<CertificateRequestSanIp>
    {
        public void Configure(EntityTypeBuilder<CertificateRequestSanIp> builder)
        {
            builder.ToTable("CertificateRequestSanIp");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.RequestId).IsRequired();
            builder.Property(x => x.IpAddress).IsRequired().HasMaxLength(45);
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasOne(x => x.Request)
                   .WithMany(x => x.SanIps)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}