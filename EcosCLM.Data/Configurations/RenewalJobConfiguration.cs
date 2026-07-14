using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class RenewalJobConfiguration : IEntityTypeConfiguration<RenewalJob>
    {
        public void Configure(EntityTypeBuilder<RenewalJob> builder)
        {
            builder.ToTable("RenewalJob");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.CertificateId).IsRequired();
            builder.Property(x => x.ScheduledAt).IsRequired();
            builder.Property(x => x.DueAt).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("SCHEDULED");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasOne(x => x.Certificate)
                   .WithMany()
                   .HasForeignKey(x => x.CertificateId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}