using EcosCLM.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class HsmClusterConfiguration : IEntityTypeConfiguration<HsmCluster>
    {
        public void Configure(EntityTypeBuilder<HsmCluster> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ToTable("HsmCluster");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Vendor).HasMaxLength(100);
            builder.Property(x => x.Model).HasMaxLength(100);
            builder.Property(x => x.PartitionLabel).HasMaxLength(150);
            builder.Property(x => x.EndpointRef).HasMaxLength(500);
            builder.Property(x => x.FipsLevel).HasMaxLength(100);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("ACTIVE");
            builder.Property(x => x.MetadataJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasMany(x => x.Keys)
                   .WithOne(x => x.HsmCluster)
                   .HasForeignKey(x => x.HsmClusterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}