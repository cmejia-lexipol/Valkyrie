using AutoMapper;
using Valkyrie.Application.Common.DTOs;
using Valkyrie.Domain.Interfaces;
using MediatR;

namespace Valkyrie.Application.Features.FieldTypes.Queries.GetAllFieldTypes;

public class GetAllFieldTypesQueryHandler : IRequestHandler<GetAllFieldTypesQuery, IEnumerable<FieldTypeDto>>
{
    private readonly IFieldTypeRepository _fieldTypeRepository;
    private readonly IMapper _mapper;

    public GetAllFieldTypesQueryHandler(IFieldTypeRepository fieldTypeRepository, IMapper mapper)
    {
        _fieldTypeRepository = fieldTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FieldTypeDto>> Handle(GetAllFieldTypesQuery request, CancellationToken cancellationToken)
    {
        var fieldTypes = await _fieldTypeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<FieldTypeDto>>(fieldTypes);
    }
} 