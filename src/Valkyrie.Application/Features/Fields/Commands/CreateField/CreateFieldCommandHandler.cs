using MediatR;
using Valkyrie.Application.Common.Mappings;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Application.Common.DTOs;
using Microsoft.Extensions.Logging;

namespace Valkyrie.Application.Features.Fields.Commands.CreateField;

public class CreateFieldCommandHandler : IRequestHandler<CreateFieldCommand, FieldDto>
{
    private readonly IFieldRepository _fieldRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IFieldTypeRepository _fieldTypeRepository;
    private readonly FieldMapper _fieldMapper;
    private readonly FieldCommandMapper _fieldCommandMapper;
    private readonly ILogger<CreateFieldCommandHandler> _logger;

    public CreateFieldCommandHandler(
        IFieldRepository fieldRepository,
        ICategoryRepository categoryRepository,
        IFieldTypeRepository fieldTypeRepository,
        FieldMapper fieldMapper,
        FieldCommandMapper fieldCommandMapper,
        ILogger<CreateFieldCommandHandler> logger)
    {
        _fieldRepository = fieldRepository;
        _categoryRepository = categoryRepository;
        _fieldTypeRepository = fieldTypeRepository;
        _fieldMapper = fieldMapper;
        _fieldCommandMapper = fieldCommandMapper;
        _logger = logger;
    }

    public async Task<FieldDto> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new field with name: {FieldName} and label: {FieldLabel}",
            request.Name, request.Label);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("Field creation failed: Name is required");
            throw new ArgumentException("Field name is required", nameof(request.Name));
        }

        if (string.IsNullOrWhiteSpace(request.Label))
        {
            _logger.LogWarning("Field creation failed: Label is required");
            throw new ArgumentException("Field label is required", nameof(request.Label));
        }

        try
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Field creation failed: CategoryId {CategoryId} does not exist", request.CategoryId);
                throw new ArgumentException($"Category with ID {request.CategoryId} does not exist.", nameof(request.CategoryId));
            }

            var fieldType = await _fieldTypeRepository.GetByIdAsync(request.FieldTypeId);
            if (fieldType == null)
            {
                _logger.LogWarning("Field creation failed: FieldTypeId {FieldTypeId} does not exist", request.FieldTypeId);
                throw new ArgumentException($"FieldType with ID {request.FieldTypeId} does not exist.", nameof(request.FieldTypeId));
            }

            var field = _fieldCommandMapper.ToEntity(request);
            field.Category = category;
            field.FieldType = fieldType;

            _logger.LogDebug("Mapped command to field entity: {@FieldEntity}", field);

            var result = await _fieldRepository.CreateAsync(field);
            _logger.LogInformation("Successfully created field with ID: {FieldId}", result.FieldId);

            var dto = _fieldMapper.ToDto(result);
            _logger.LogDebug("Mapped field entity to DTO: {@FieldDto}", dto);

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating field with name: {FieldName}. Error: {ErrorMessage}",
                request.Name, ex.Message);
            throw;
        }
    }
}