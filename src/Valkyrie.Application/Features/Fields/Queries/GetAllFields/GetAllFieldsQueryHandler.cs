using Valkyrie.Application.Common.DTOs;
using Valkyrie.Application.Features.Fields.Commands.CreateField;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Valkyrie.Application.Common.Mappings;

namespace Valkyrie.Application.Features.Fields.Queries.GetAllFields;

public class GetAllFieldsQueryHandler : IRequestHandler<GetAllFieldsQuery, IEnumerable<FieldDto>>
{
    private readonly IFieldRepository _fieldRepository;
    private readonly FieldMapper _fieldMapper;
    private readonly ILogger<GetAllFieldsQueryHandler> _logger;

    public GetAllFieldsQueryHandler(IFieldRepository fieldRepository, FieldMapper fieldMapper, ILogger<GetAllFieldsQueryHandler> logger)
    {
        _fieldRepository = fieldRepository;
        _fieldMapper = fieldMapper;
        _logger = logger;
    }

    public async Task<IEnumerable<FieldDto>> Handle(GetAllFieldsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all Fields");
        var fields = await _fieldRepository.GetAllAsync();
        return fields.Select(_fieldMapper.ToDto);
    }
}