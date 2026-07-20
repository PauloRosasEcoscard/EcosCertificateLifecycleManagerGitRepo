using EcosCLM.Domain.Entities.Deployment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class DeploymentTargetConfiguration : IEntityTypeConfiguration<DeploymentTarget>
    {
        public void Configure(EntityTypeBuilder<DeploymentTarget> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ToTable("DeploymentTarget");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.ApplicationId);
            builder.Property(x => x.EnvironmentId);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TargetType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.EndpointRef).HasMaxLength(500);
            builder.Property(x => x.SecretRef).HasMaxLength(500);
            builder.Property(x => x.AutomationEnabled).IsRequired().HasDefaultValue((short)0);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("ACTIVE");
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}