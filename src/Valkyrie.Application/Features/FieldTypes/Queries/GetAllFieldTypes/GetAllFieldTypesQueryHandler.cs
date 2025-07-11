using Valkyrie.Application.Common.DTOs;
using Valkyrie.Domain.Interfaces;
using MediatR;
using Valkyrie.Application.Common.Mappings;

namespace Valkyrie.Application.Features.FieldTypes.Queries.GetAllFieldTypes;

public class GetAllFieldTypesQueryHandler : IRequestHandler<GetAllFieldTypesQuery, IEnumerable<FieldTypeDto>>
{
    private readonly IFieldTypeRepository _fieldTypeRepository;
    private readonly FieldTypeMapper _fieldTypeMapper;

    public GetAllFieldTypesQueryHandler(IFieldTypeRepository fieldTypeRepository, FieldTypeMapper fieldTypeMapper)
    {
        _fieldTypeRepository = fieldTypeRepository;
        _fieldTypeMapper = fieldTypeMapper;
    }

    public async Task<IEnumerable<FieldTypeDto>> Handle(GetAllFieldTypesQuery request, CancellationToken cancellationToken)
    {
        var fieldTypes = await _fieldTypeRepository.GetAllAsync();
        return fieldTypes.Select(_fieldTypeMapper.ToDto);
    }
}