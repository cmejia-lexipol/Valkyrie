using MediatR;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.FieldTypes.Queries.GetAllFieldTypes;

public record GetAllFieldTypesQuery : IRequest<IEnumerable<FieldTypeDto>>
{
}