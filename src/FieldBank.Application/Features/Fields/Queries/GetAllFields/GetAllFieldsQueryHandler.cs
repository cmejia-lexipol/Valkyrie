using AutoMapper;
using FieldBank.Application.Common.DTOs;
using FieldBank.Application.Features.Fields.Commands.CreateField;
using FieldBank.Domain.Entities;
using FieldBank.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FieldBank.Application.Features.Fields.Queries.GetAllFields;

public class GetAllFieldsQueryHandler : IRequestHandler<GetAllFieldsQuery, IEnumerable<FieldDto>>
{
    private readonly IFieldRepository _fieldRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllFieldsQueryHandler> _logger;

    public GetAllFieldsQueryHandler(IFieldRepository fieldRepository, IMapper mapper, ILogger<GetAllFieldsQueryHandler> logger)
    {
        _fieldRepository = fieldRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<FieldDto>> Handle(GetAllFieldsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all Fields");
        var fields = await _fieldRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<FieldDto>>(fields);
    }
} 