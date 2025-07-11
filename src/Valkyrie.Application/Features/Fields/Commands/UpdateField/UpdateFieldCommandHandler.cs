using MediatR;
using Valkyrie.Application.Common.Mappings;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Fields.Commands.UpdateField;

public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, FieldDto>
{
    private readonly IFieldRepository _fieldRepository;
    private readonly FieldMapper _fieldMapper;
    private readonly FieldCommandMapper _fieldCommandMapper;

    public UpdateFieldCommandHandler(IFieldRepository fieldRepository, FieldMapper fieldMapper, FieldCommandMapper fieldCommandMapper)
    {
        _fieldRepository = fieldRepository;
        _fieldMapper = fieldMapper;
        _fieldCommandMapper = fieldCommandMapper;
    }

    public async Task<FieldDto> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("Field ID must be greater than 0", nameof(request.Id));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Field name is required", nameof(request.Name));

        if (string.IsNullOrWhiteSpace(request.Label))
            throw new ArgumentException("Field label is required", nameof(request.Label));

        var existingField = await _fieldRepository.GetByIdAsync(request.Id);
        if (existingField == null)
            throw new InvalidOperationException($"Field with ID {request.Id} not found");

        // Map the command to the existing field, preserving audit fields
        _fieldCommandMapper.UpdateEntity(request, existingField);

        var result = await _fieldRepository.UpdateAsync(existingField);
        return _fieldMapper.ToDto(result);
    }
}