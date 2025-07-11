using MediatR;
using Valkyrie.Application.Common.Mappings;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryMapper _categoryMapper;
    private readonly CategoryCommandMapper _categoryCommandMapper;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, CategoryMapper categoryMapper, CategoryCommandMapper categoryCommandMapper)
    {
        _categoryRepository = categoryRepository;
        _categoryMapper = categoryMapper;
        _categoryCommandMapper = categoryCommandMapper;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(request.Id);
        if (existingCategory == null)
            throw new InvalidOperationException($"Category with ID {request.Id} not found");

        _categoryCommandMapper.UpdateEntity(request, existingCategory);
        var result = await _categoryRepository.UpdateAsync(existingCategory);
        return _categoryMapper.ToDto(result);
    }
}