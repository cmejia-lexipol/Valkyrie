using Valkyrie.Application.Common.DTOs;
using Valkyrie.Domain.Interfaces;
using MediatR;
using Valkyrie.Application.Common.Mappings;

namespace Valkyrie.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryMapper _categoryMapper;

    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository, CategoryMapper categoryMapper)
    {
        _categoryRepository = categoryRepository;
        _categoryMapper = categoryMapper;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(_categoryMapper.ToDto);
    }
}