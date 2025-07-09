using MediatR;

namespace Valkyrie.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand : IRequest<Unit>
{
    public int Id { get; init; }
}