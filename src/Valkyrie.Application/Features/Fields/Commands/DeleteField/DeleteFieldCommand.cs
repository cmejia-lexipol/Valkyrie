using MediatR;

namespace Valkyrie.Application.Features.Fields.Commands.DeleteField;

public record DeleteFieldCommand : IRequest<Unit>
{
    public int Id { get; init; }
}