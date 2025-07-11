using Riok.Mapperly.Abstractions;
using Valkyrie.Domain.Entities;
using Valkyrie.Application.Features.Fields.Commands.CreateField;
using Valkyrie.Application.Features.Fields.Commands.UpdateField;

namespace Valkyrie.Application.Common.Mappings;

[Mapper]
public partial class FieldCommandMapper
{
    public partial Field ToEntity(CreateFieldCommand command);
    public partial void UpdateEntity(UpdateFieldCommand command, Field entity);
} 