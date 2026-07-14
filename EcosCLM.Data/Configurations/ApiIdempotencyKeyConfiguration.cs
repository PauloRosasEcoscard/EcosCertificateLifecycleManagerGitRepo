using EcosCLM.Domain.Entities.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcosCLM.Data.Configurations
{
    public class ApiIdempotencyKeyConfiguration : IEntityTypeConfiguration<ApiIdempotencyKey>
    {
        public void Configure(EntityTypeBuilder<ApiIdempotencyKey> builder)
        {
            builder.ToTable("ApiIdempotencyKey");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.Key).IsRequired().HasMaxLength(255);
            builder.Property(x => x.ResponseJson).IsRequired().HasDefaultValue("{}");
            builder.Property(x => x.ExpiresAt).IsRequired();

            builder.HasIndex(x => new { x.CustomerId, x.Key }).IsUnique();
        }
    }
}