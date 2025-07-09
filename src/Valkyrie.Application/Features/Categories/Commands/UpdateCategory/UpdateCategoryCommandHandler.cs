using MediatR;
using AutoMapper;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(request.Id);
        if (existingCategory == null)
            throw new InvalidOperationException($"Category with ID {request.Id} not found");

        _mapper.Map(request, existingCategory);
        var result = await _categoryRepository.UpdateAsync(existingCategory);
        return _mapper.Map<CategoryDto>(result);
    }
}