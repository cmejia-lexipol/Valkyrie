namespace Valkyrie.Domain.Entities;

public class Category : BaseEntity
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Rank { get; set; }
    public ICollection<Field> Fields { get; set; } = new List<Field>();
}