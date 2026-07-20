using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class CaOrderConfiguration : IEntityTypeConfiguration<CaOrder>
    {
        public void Configure(EntityTypeBuilder<CaOrder> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ToTable("CaOrder");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.RequestId).IsRequired();
            builder.Property(x => x.CaId).IsRequired();
            builder.Property(x => x.ExternalOrderId).HasMaxLength(255);
            builder.Property(x => x.ExternalCertificateId).HasMaxLength(255);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("CREATED");
            builder.Property(x => x.SubmittedAt);
            builder.Property(x => x.CompletedAt);
            builder.Property(x => x.ErrorCode).HasMaxLength(100);
            builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
            builder.Property(x => x.RawResponseRef).HasMaxLength(500);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasOne(x => x.Request)
                   .WithMany()
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CertificateAuthority)
                   .WithMany()
                   .HasForeignKey(x => x.CaId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}