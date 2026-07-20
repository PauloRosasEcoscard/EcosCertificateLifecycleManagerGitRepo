using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class CertificateRequestSanDnsConfiguration : IEntityTypeConfiguration<CertificateRequestSanDns>
    {
        public void Configure(EntityTypeBuilder<CertificateRequestSanDns> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ToTable("CertificateRequestSanDns");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.RequestId).IsRequired();
            builder.Property(x => x.DnsName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasOne(x => x.Request)
                   .WithMany(x => x.SanDns)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}