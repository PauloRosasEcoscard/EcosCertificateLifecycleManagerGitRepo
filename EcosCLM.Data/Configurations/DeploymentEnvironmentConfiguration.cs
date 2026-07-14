using EcosCLM.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class DeploymentEnvironmentConfiguration : IEntityTypeConfiguration<DeploymentEnvironment>
    {
        public void Configure(EntityTypeBuilder<DeploymentEnvironment> builder)
        {
            builder.ToTable("DeploymentEnvironment");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasMany(x => x.DeploymentTargets)
                   .WithOne()
                   .HasForeignKey(x => x.EnvironmentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}