using Riok.Mapperly.Abstractions;
using Valkyrie.Domain.Entities;
using Valkyrie.Application.Features.Categories.Commands.CreateCategory;
using Valkyrie.Application.Features.Categories.Commands.UpdateCategory;

namespace Valkyrie.Application.Common.Mappings;

[Mapper]
public partial class CategoryCommandMapper
{
    public partial Category ToEntity(CreateCategoryCommand command);
    public partial void UpdateEntity(UpdateCategoryCommand command, Category entity);
} 