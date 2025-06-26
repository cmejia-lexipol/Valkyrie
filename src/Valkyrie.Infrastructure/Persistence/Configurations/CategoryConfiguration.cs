using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valkyrie.Domain.Entities;

namespace Valkyrie.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.CategoryId);
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(c => c.Description)
            .HasMaxLength(500);
        builder.Property(c => c.Rank)
            .IsRequired();

        // Auditing fields (if in BaseEntity)
        builder.Property(c => c.CreatedBy).HasMaxLength(100);
        builder.Property(c => c.ModifiedBy).HasMaxLength(100);
        builder.Property(c => c.CreatedDate).IsRequired();
        builder.Property(c => c.ModifiedDate);

        builder.HasMany(c => c.Fields)
            .WithOne(f => f.Category)
            .HasForeignKey(f => f.CategoryId)
            .HasPrincipalKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 