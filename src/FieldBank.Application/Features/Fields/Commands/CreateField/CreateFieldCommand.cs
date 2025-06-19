using MediatR;
using FieldBank.Application.Common.DTOs;

namespace FieldBank.Application.Features.Fields.Commands.CreateField;

public record CreateFieldCommand : IRequest<FieldDto>
{
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Description { get; init; }
} 