using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valkyrie.Domain.Entities;

namespace Valkyrie.Infrastructure.Persistence.Configurations;

public class FieldsConfiguration : BaseEntityConfiguration<Field>
{
    public override void Configure(EntityTypeBuilder<Field> builder)
    {
        base.Configure(builder);

        builder.ToTable("Fields");

        builder.HasKey(f => f.FieldId);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Label)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        // Audit fields configuration
        builder.Property(f => f.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(f => f.ModifiedDate);

        builder.Property(f => f.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("System");

        builder.Property(f => f.ModifiedBy)
            .HasMaxLength(100);

        builder.HasOne(f => f.FieldType)
            .WithMany()
            .HasForeignKey(f => f.FieldTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}