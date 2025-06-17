using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FieldBank.Domain.Entities;

namespace FieldBank.Infrastructure.Persistence.Configurations
{
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            // Audit fields configuration
            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.ModifiedDate);

            builder.Property(e => e.CreatedBy)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("System");

            builder.Property(e => e.ModifiedBy)
                .HasMaxLength(100);
        }
    }
} 