namespace Valkyrie.Application.Common.DTOs;

public record FieldTypeDto
{
    public int FieldTypeId { get; init; }
    public string Type { get; init; } = string.Empty;
    public string? Structure { get; init; }
}