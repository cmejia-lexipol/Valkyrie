namespace Valkyrie.Domain.Entities;

public class Field : BaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Label { get; set; }
    public string? Description { get; set; }
}

