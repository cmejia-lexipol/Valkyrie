using MediatR;
using Valkyrie.Application.Common.Mappings;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryMapper _categoryMapper;
    private readonly CategoryCommandMapper _categoryCommandMapper;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, CategoryMapper categoryMapper, CategoryCommandMapper categoryCommandMapper)
    {
        _categoryRepository = categoryRepository;
        _categoryMapper = categoryMapper;
        _categoryCommandMapper = categoryCommandMapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _categoryCommandMapper.ToEntity(request);
        var result = await _categoryRepository.CreateAsync(category);
        return _categoryMapper.ToDto(result);
    }
}