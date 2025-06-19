namespace FieldBank.Application.Common.DTOs;

public record FieldDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime? ModifiedDate { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public string? ModifiedBy { get; init; }
} 