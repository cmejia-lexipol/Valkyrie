using MediatR;
using AutoMapper;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Application.Common.DTOs;
using Microsoft.Extensions.Logging;

namespace Valkyrie.Application.Features.Fields.Commands.CreateField;

public class CreateFieldCommandHandler : IRequestHandler<CreateFieldCommand, FieldDto>
{
    private readonly IFieldRepository _fieldRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateFieldCommandHandler> _logger;

    public CreateFieldCommandHandler(IFieldRepository fieldRepository, IMapper mapper, ILogger<CreateFieldCommandHandler> logger)
    {
        _fieldRepository = fieldRepository;
        _mapper = mapper;
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
            var field = _mapper.Map<Field>(request);
            _logger.LogDebug("Mapped command to field entity: {@FieldEntity}", field);

            var result = await _fieldRepository.CreateAsync(field);
            _logger.LogInformation("Successfully created field with ID: {FieldId}", result.FieldId);
            
            var dto = _mapper.Map<FieldDto>(result);
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