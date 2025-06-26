using MediatR;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Fields.Queries.GetAllFields;

public record GetAllFieldsQuery : IRequest<IEnumerable<FieldDto>>
{
}