using MediatR;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
{
} 