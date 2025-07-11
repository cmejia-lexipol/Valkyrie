using MediatR;
using Valkyrie.Application.Common.Mappings;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Fields.Queries.GetFieldById;

public class GetFieldByIdQueryHandler : IRequestHandler<GetFieldByIdQuery, FieldDto?>
{
    private readonly IFieldRepository _fieldRepository;
    private readonly FieldMapper _fieldMapper;

    public GetFieldByIdQueryHandler(IFieldRepository fieldRepository, FieldMapper fieldMapper)
    {
        _fieldRepository = fieldRepository;
        _fieldMapper = fieldMapper;
    }

    public async Task<FieldDto?> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("Field ID must be greater than 0", nameof(request.Id));

        var field = await _fieldRepository.GetByIdAsync(request.Id);
        return field != null ? _fieldMapper.ToDto(field) : null;
    }
}