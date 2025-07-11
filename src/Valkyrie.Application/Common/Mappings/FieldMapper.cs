using Riok.Mapperly.Abstractions;
using Valkyrie.Domain.Entities;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Common.Mappings;

[Mapper]
public partial class FieldMapper
{
    public partial FieldDto ToDto(Field field);
    public partial Field ToEntity(FieldDto dto);
} 