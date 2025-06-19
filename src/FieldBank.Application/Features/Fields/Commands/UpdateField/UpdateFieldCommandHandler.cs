using MediatR;
using AutoMapper;
using FieldBank.Domain.Entities;
using FieldBank.Domain.Interfaces;
using FieldBank.Application.Common.DTOs;

namespace FieldBank.Application.Features.Fields.Commands.UpdateField;

public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, FieldDto>
{
    private readonly IFieldRepository _fieldRepository;
    private readonly IMapper _mapper;

    public UpdateFieldCommandHandler(IFieldRepository fieldRepository, IMapper mapper)
    {
        _fieldRepository = fieldRepository;
        _mapper = mapper;
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
        _mapper.Map(request, existingField);

        var result = await _fieldRepository.UpdateAsync(existingField);
        return _mapper.Map<FieldDto>(result);
    }
} 