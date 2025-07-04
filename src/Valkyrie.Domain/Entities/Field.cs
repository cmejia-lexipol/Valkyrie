namespace Valkyrie.Domain.Entities;

public class Field : BaseEntity
{
    public int FieldId { get; set; }
    public required string Name { get; set; }
    public required string Label { get; set; }
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int FieldTypeId { get; set; }
    public FieldType FieldType { get; set; } = null!;
}

