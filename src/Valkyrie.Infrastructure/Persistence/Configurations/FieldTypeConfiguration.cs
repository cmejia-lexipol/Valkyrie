using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Enums;

namespace Valkyrie.Infrastructure.Persistence.Configurations;

public class FieldTypeConfiguration : IEntityTypeConfiguration<FieldType>
{
    public void Configure(EntityTypeBuilder<FieldType> builder)
    {
        builder.ToTable("FieldTypes");

        builder.HasKey(ft => ft.FieldTypeId);

        builder.Property(ft => ft.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(ft => ft.Structure)
            .IsRequired()
            .HasColumnType("jsonb");

        // Seed data
        builder.HasData(
            new FieldType
            {
                FieldTypeId = (int)FieldTypeEnum.Date,
                Type = FieldTypeEnum.Date,
                Structure = "{}"
            },
            new FieldType
            {
                FieldTypeId = (int)FieldTypeEnum.Time,
                Type = FieldTypeEnum.Time,
                Structure = "{}"
            },
            new FieldType
            {
                FieldTypeId = (int)FieldTypeEnum.Number,
                Type = FieldTypeEnum.Number,
                Structure = "{}"
            },
            new FieldType
            {
                FieldTypeId = (int)FieldTypeEnum.Text,
                Type = FieldTypeEnum.Text,
                Structure = "{}"
            },
            new FieldType
            {
                FieldTypeId = (int)FieldTypeEnum.Boolean,
                Type = FieldTypeEnum.Boolean,
                Structure = "{}"
            },
            new FieldType
            {
                FieldTypeId = (int)FieldTypeEnum.SingleSelect,
                Type = FieldTypeEnum.SingleSelect,
                Structure = "{}"
            }
        );
    }
}