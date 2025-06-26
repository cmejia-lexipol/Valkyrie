using MediatR;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Fields.Commands.UpdateField;

public record UpdateFieldCommand : IRequest<FieldDto>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int CategoryId { get; init; }
}