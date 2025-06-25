using MediatR;
using FieldBank.Application.Common.DTOs;

namespace FieldBank.Application.Features.Fields.Queries.GetAllFields;

public record GetAllFieldsQuery : IRequest<IEnumerable<FieldDto>>
{
}