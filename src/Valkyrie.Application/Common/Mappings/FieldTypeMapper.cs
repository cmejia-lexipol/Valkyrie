using Riok.Mapperly.Abstractions;
using Valkyrie.Domain.Entities;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Common.Mappings;

[Mapper]
public partial class FieldTypeMapper
{
    public partial FieldTypeDto ToDto(FieldType fieldType);
    public partial FieldType ToEntity(FieldTypeDto dto);
} 