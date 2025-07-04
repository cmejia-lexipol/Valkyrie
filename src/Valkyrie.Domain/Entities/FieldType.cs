using Valkyrie.Domain.Enums;

namespace Valkyrie.Domain.Entities;

public class FieldType
{
    public int FieldTypeId { get; set; }
    public FieldTypeEnum Type { get; set; }
    public string Structure { get; set; } = string.Empty;
}
