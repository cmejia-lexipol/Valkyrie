using MediatR;
using FieldBank.Application.Common.DTOs;

namespace FieldBank.Application.Features.Fields.Commands.UpdateField;

public record UpdateFieldCommand : IRequest<FieldDto>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Description { get; init; }
}