using Riok.Mapperly.Abstractions;
using Valkyrie.Domain.Entities;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Common.Mappings;

[Mapper]
public partial class CategoryMapper
{
    public partial CategoryDto ToDto(Category category);
    public partial Category ToEntity(CategoryDto dto);
} 