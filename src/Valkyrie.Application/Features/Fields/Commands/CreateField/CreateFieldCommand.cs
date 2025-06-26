using MediatR;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Fields.Commands.CreateField;

public record CreateFieldCommand : IRequest<FieldDto>
{
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Description { get; init; }
}