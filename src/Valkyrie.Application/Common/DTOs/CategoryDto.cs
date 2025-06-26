namespace Valkyrie.Application.Common.DTOs;

public record CategoryDto
{
    public int CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Rank { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime? ModifiedDate { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public string? ModifiedBy { get; init; }
} 