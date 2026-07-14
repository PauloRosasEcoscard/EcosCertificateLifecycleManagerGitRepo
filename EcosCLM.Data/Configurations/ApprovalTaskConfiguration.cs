using EcosCLM.Domain.Entities.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class ApprovalTaskConfiguration : IEntityTypeConfiguration<ApprovalTask>
    {
        public void Configure(EntityTypeBuilder<ApprovalTask> builder)
        {
            builder.ToTable("ApprovalTask");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.RequestId).IsRequired();
            builder.Property(x => x.StepOrder).IsRequired().HasDefaultValue(1);
            builder.Property(x => x.ApproverRoleId);
            builder.Property(x => x.ApproverUserId);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue("PENDING");
            builder.Property(x => x.DecisionComment).HasMaxLength(1000);
            builder.Property(x => x.DecidedBy);
            builder.Property(x => x.DecidedAt);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasOne(x => x.Request)
                   .WithMany(x => x.ApprovalTasks)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}