using EcosCLM.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class HsmKeyRefConfiguration : IEntityTypeConfiguration<HsmKeyRef>
    {
        public void Configure(EntityTypeBuilder<HsmKeyRef> builder)
        {
            builder.ToTable("HsmKeyRef");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.HsmClusterId).IsRequired();
            builder.Property(x => x.KeyLabel).IsRequired().HasMaxLength(255);
            builder.Property(x => x.KeyHandle).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Algorithm).IsRequired().HasMaxLength(50);
            builder.Property(x => x.KeySize);
            builder.Property(x => x.CurveName).HasMaxLength(50);
            builder.Property(x => x.Extractable).IsRequired().HasDefaultValue((short)0);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("ACTIVE");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasOne(x => x.HsmCluster)
                   .WithMany(x => x.Keys)
                   .HasForeignKey(x => x.HsmClusterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}